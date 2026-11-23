using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Enums;
using MedicalAppointments.Persistence.Citas.Context;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Persistence.Extensions;

/// <summary>
/// Alinea el catálogo Estados_Cita con los estados oficiales sin asumir identificadores previos.
/// La migración se hace por código (con sinónimos) y actualiza las citas existentes en la misma
/// operación, conservando el CitaID.
/// </summary>
public static class EstadoCitaCatalogExtensions
{
    private static readonly (int Id, string Codigo, string Nombre)[]
        EstadosOficiales =
        {
            ((int)EstadoCitaEnum.Agendada, "AGENDADA", "Agendada"),
            ((int)EstadoCitaEnum.Citada, "CITADA", "Citada"),
            ((int)EstadoCitaEnum.PendienteResultados, "PENDIENTE_RESULTADOS", "Pendiente de Resultados"),
            ((int)EstadoCitaEnum.Concluida, "CONCLUIDA", "Concluida"),
            ((int)EstadoCitaEnum.Cancelada, "CANCELADA", "Cancelada")
        };

    private static readonly Dictionary<string, string> Sinonimos = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AGENDADA"] = "AGENDADA",
        ["SCHEDULED"] = "AGENDADA",
        ["CITADA"] = "CITADA",
        ["CONFIRMADA"] = "CITADA",
        ["CONFIRMED"] = "CITADA",
        ["PENDIENTE_RESULTADOS"] = "PENDIENTE_RESULTADOS",
        ["PENDING_RESULTS"] = "PENDIENTE_RESULTADOS",
        ["CONCLUIDA"] = "CONCLUIDA",
        ["ATENDIDA"] = "CONCLUIDA",
        ["COMPLETADA"] = "CONCLUIDA",
        ["FINALIZADA"] = "CONCLUIDA",
        ["CONCLUDED"] = "CONCLUIDA",
        ["CANCELADA"] = "CANCELADA",
        ["CANCELLED"] = "CANCELADA"
    };

    public static async Task MigrarCatalogoEstadosOficialesAsync(
        this CitasDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var esSqlServer = dbContext.Database.IsSqlServer();

        if (esSqlServer)
        {
            // EnsureCreated no altera bases de datos ya existentes; estas columnas son
            // snapshots obligatorios del catálogo de médicos/especialidades.
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                IF COL_LENGTH(N'Citas', N'NombreMedico') IS NULL
                    ALTER TABLE [Citas] ADD [NombreMedico] nvarchar(150) NULL;
                IF COL_LENGTH(N'Citas', N'Especialidad') IS NULL
                    ALTER TABLE [Citas] ADD [Especialidad] nvarchar(150) NULL;
                """,
                cancellationToken);
        }

        var estados = await dbContext.EstadosCita.ToListAsync(cancellationToken);
        var citas = await dbContext.Citas.ToListAsync(cancellationToken);

        var codigoOriginalPorId = estados.ToDictionary(
            e => e.EstadoCitaID,
            e => Reconocer(e.Codigo, e.Nombre));

        var idOficialPorCodigo = EstadosOficiales.ToDictionary(
            e => e.Codigo,
            e => e.Id,
            StringComparer.OrdinalIgnoreCase);

        var idAgendada = (int)EstadoCitaEnum.Agendada;
        var idsOficiales = EstadosOficiales.Select(e => e.Id).ToHashSet();
        var oficialesPorId = EstadosOficiales.ToDictionary(e => e.Id);

        foreach (var cita in citas)
        {
            var codigo = codigoOriginalPorId.TryGetValue(cita.EstadoCitaID, out var reconocido)
                ? reconocido
                : null;

            var destino = codigo is not null && idOficialPorCodigo.TryGetValue(codigo, out var id)
                ? id
                : idAgendada;

            cita.EstadoCitaID = destino;
        }

        var faltantes = EstadosOficiales
            .Where(oficial => estados.All(e => e.EstadoCitaID != oficial.Id))
            .ToArray();

        if (esSqlServer)
        {
            await MigrarCatalogoSqlServerAsync(dbContext, estados, faltantes, idsOficiales, cancellationToken);
        }
        else
        {
            await MigrarCatalogoEnMemoriaAsync(dbContext, estados, faltantes, idsOficiales, oficialesPorId, cancellationToken);
        }
    }

    private static async Task MigrarCatalogoEnMemoriaAsync(
        CitasDbContext dbContext,
        List<EstadoCita> estados,
        IReadOnlyCollection<(int Id, string Codigo, string Nombre)> faltantes,
        HashSet<int> idsOficiales,
        Dictionary<int, (int Id, string Codigo, string Nombre)> oficialesPorId,
        CancellationToken cancellationToken)
    {
        foreach (var oficial in faltantes)
        {
            var estado = new EstadoCita
            {
                EstadoCitaID = oficial.Id,
                Codigo = oficial.Codigo,
                Nombre = oficial.Nombre
            };

            dbContext.EstadosCita.Add(estado);
            estados.Add(estado);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var estado in estados)
        {
            if (idsOficiales.Contains(estado.EstadoCitaID))
            {
                var oficial = oficialesPorId[estado.EstadoCitaID];
                estado.Codigo = oficial.Codigo;
                estado.Nombre = oficial.Nombre;
            }
            else
            {
                estado.Codigo = $"LEGACY_{estado.EstadoCitaID}";
                estado.Nombre = $"Legado {estado.EstadoCitaID}";
            }
        }

        dbContext.EstadosCita.RemoveRange(estados.Where(e => !idsOficiales.Contains(e.EstadoCitaID)));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task MigrarCatalogoSqlServerAsync(
        CitasDbContext dbContext,
        List<EstadoCita> estados,
        IReadOnlyCollection<(int Id, string Codigo, string Nombre)> faltantes,
        HashSet<int> idsOficiales,
        CancellationToken cancellationToken)
    {
        await using var transaccion = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (faltantes.Count > 0)
                await InsertarEstadosSqlServerAsync(dbContext, faltantes, cancellationToken);

            // Las citas se reasignan antes de tocar el catálogo para no romper la FK existente.
            await dbContext.SaveChangesAsync(cancellationToken);

            foreach (var estado in estados)
                dbContext.Entry(estado).State = EntityState.Detached;

            var ids = string.Join(", ", idsOficiales.OrderBy(id => id)
                .Select(id => id.ToString(System.Globalization.CultureInfo.InvariantCulture)));

            var legacySql = "UPDATE [Estados_Cita] "
                            + "SET [Codigo] = CONCAT(N'LEGACY_', [EstadoCitaID]), "
                            + "[Nombre] = CONCAT(N'Legado ', [EstadoCitaID]) "
                            + "WHERE [EstadoCitaID] NOT IN (" + ids + ");";
            await dbContext.Database.ExecuteSqlRawAsync(legacySql, cancellationToken);

            var temporalSql = "UPDATE [Estados_Cita] "
                              + "SET [Codigo] = CONCAT(N'TMP_OFICIAL_', [EstadoCitaID]) "
                              + "WHERE [EstadoCitaID] IN (" + ids + ");";
            await dbContext.Database.ExecuteSqlRawAsync(temporalSql, cancellationToken);

            var parametros = new List<object>();
            var casosCodigo = new List<string>();
            var casosNombre = new List<string>();

            foreach (var oficial in EstadosOficiales)
            {
                var indiceCodigo = parametros.Count;
                parametros.Add(oficial.Codigo);
                var indiceNombre = parametros.Count;
                parametros.Add(oficial.Nombre);

                casosCodigo.Add($"WHEN {oficial.Id} THEN {{{indiceCodigo}}}");
                casosNombre.Add($"WHEN {oficial.Id} THEN {{{indiceNombre}}}");
            }

            var definitivoSql = "UPDATE [Estados_Cita] SET "
                                + "[Codigo] = CASE [EstadoCitaID] " + string.Join(" ", casosCodigo) + " END, "
                                + "[Nombre] = CASE [EstadoCitaID] " + string.Join(" ", casosNombre) + " END "
                                + "WHERE [EstadoCitaID] IN (" + ids + ");";
            await dbContext.Database.ExecuteSqlRawAsync(definitivoSql, parametros, cancellationToken);

            await dbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM [Estados_Cita] WHERE [EstadoCitaID] NOT IN (" + ids + ");",
                cancellationToken);

            var maxId = idsOficiales.DefaultIfEmpty(0).Max();
            var reseedSql = "DBCC CHECKIDENT(N'Estados_Cita', RESEED, "
                            + maxId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                            + ");";
            await dbContext.Database.ExecuteSqlRawAsync(reseedSql, cancellationToken);

            await transaccion.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaccion.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task InsertarEstadosSqlServerAsync(
        CitasDbContext dbContext,
        IReadOnlyCollection<(int Id, string Codigo, string Nombre)> estados,
        CancellationToken cancellationToken)
    {
        var parametros = new List<object>();
        var filas = new List<string>();

        foreach (var estado in estados)
        {
            var p = parametros.Count;
            parametros.Add(estado.Id);
            parametros.Add($"TMP_OFICIAL_{estado.Id}");
            parametros.Add(estado.Nombre);
            filas.Add($"({{{p}}}, {{{p + 1}}}, {{{p + 2}}})");
        }

        // IDENTITY_INSERT es un estado de sesión: debe ir en el mismo lote SQL que el INSERT.
        var sql = "SET IDENTITY_INSERT [Estados_Cita] ON; "
                  + "INSERT INTO [Estados_Cita] ([EstadoCitaID], [Codigo], [Nombre]) VALUES "
                  + string.Join(", ", filas) + "; "
                  + "SET IDENTITY_INSERT [Estados_Cita] OFF;";

        await dbContext.Database.ExecuteSqlRawAsync(sql, parametros, cancellationToken);
    }

    private static string? Reconocer(string? codigo, string? nombre)
    {
        var valor = (codigo ?? nombre ?? string.Empty).Trim();
        if (Sinonimos.TryGetValue(valor, out var oficial))
            return oficial;

        var sinGuiones = valor.Replace("_", string.Empty).Replace(" ", string.Empty);
        return Sinonimos.TryGetValue(sinGuiones, out oficial) ? oficial : null;
    }
}
