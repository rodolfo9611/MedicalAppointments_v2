using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Application.Mappings;

/// <summary>
/// Conversiones entre entidades de dominio y DTOs de la API.
/// </summary>
public static class PacienteMappings
{
    /// <summary>Convierte un paciente en su DTO de salida.</summary>
    public static GetPacienteDto ToDto(Paciente paciente)
    {
        ArgumentNullException.ThrowIfNull(paciente);

        return new GetPacienteDto
        {
            PacienteID = paciente.Id,
            Nombre = paciente.Nombre,
            Apellido = paciente.Apellido,
            DocumentoIdentidad = paciente.DocumentoIdentidad,
            FechaNacimiento = paciente.FechaNacimiento,
            CodigoPaciente = paciente.CodigoPaciente,
            CorreoElectronico = paciente.CorreoElectronico,
            Telefono = paciente.Telefono,
            Historiales = paciente.Historiales.Select(ToDto).ToList()
        };
    }

    /// <summary>Convierte un evento de historial en su DTO de salida.</summary>
    public static GetHistorialMedicoDto ToDto(HistorialMedico historial)
    {
        ArgumentNullException.ThrowIfNull(historial);

        return new GetHistorialMedicoDto
        {
            Id = historial.Id,
            PacienteID = historial.PacienteId,
            TipoEvento = historial.TipoEvento,
            Descripcion = historial.Descripcion,
            EventKey = historial.EventKey,
            FechaEvento = historial.FechaEvento,
            FechaRegistro = historial.FechaRegistro,
            CitaID = historial.CitaID,
            EstadoCitaID = historial.EstadoCitaID,
            RecetaMedicaID = historial.RecetaMedicaID,
            NotificacionID = historial.NotificacionID,
            SecuenciaAtencion = historial.SecuenciaAtencion,
            FechaAnterior = historial.FechaAnterior,
            FechaNueva = historial.FechaNueva,
            MotivoReprogramacion = historial.MotivoReprogramacion,
            Diagnostico = historial.Diagnostico,
            Tratamiento = historial.Tratamiento
        };
    }

    /// <summary>Copia los campos maestros de un DTO sobre una entidad existente.</summary>
    public static void Apply(Paciente paciente, CreatePacienteDto dto)
    {
        ArgumentNullException.ThrowIfNull(paciente);
        ArgumentNullException.ThrowIfNull(dto);

        paciente.Nombre = dto.Nombre;
        paciente.Apellido = dto.Apellido;
        paciente.DocumentoIdentidad = dto.DocumentoIdentidad;
        paciente.FechaNacimiento = dto.FechaNacimiento;
        paciente.CodigoPaciente = dto.CodigoPaciente.Trim();
        paciente.CorreoElectronico = dto.CorreoElectronico;
        paciente.Telefono = dto.Telefono;
    }
}
