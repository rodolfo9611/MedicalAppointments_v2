using System.Text.Json;

namespace MedicalAppointments.RecetasMedicas.Domain.Integration;

/// <summary>
/// DTO con el subconjunto de datos de la cita que se conserva en la receta.
/// Estado se modela como JsonElement para aceptar tanto su forma textual
/// ("Concluida") como la numérica (4) según la configuración JSON del origen.
/// </summary>
public sealed class CitaSnapshotDto
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public int? MedicoID { get; set; }
    public int EstadoCitaID { get; set; }

    public JsonElement Estado { get; set; }

    public string? EstadoNombre { get; set; }
    public string? NombreMedico { get; set; }
    public string? Especialidad { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public string? Motivo { get; set; }

    public string EstadoTexto
    {
        get
        {
            if (Estado.ValueKind == JsonValueKind.String)
                return Estado.GetString() ?? string.Empty;

            if (Estado.ValueKind == JsonValueKind.Number)
                return Estado.TryGetInt32(out var numero) ? numero.ToString() : Estado.GetRawText();

            return string.Empty;
        }
    }
}
