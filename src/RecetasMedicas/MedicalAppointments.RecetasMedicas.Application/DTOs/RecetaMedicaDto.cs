namespace MedicalAppointments.RecetasMedicas.Application.DTOs;

/// <summary>Representación pública de una receta médica (incluye anuladas).</summary>
public sealed class RecetaMedicaDto
{
    public long RecetaMedicaID { get; set; }
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public string NombrePaciente { get; set; } = string.Empty;
    public DateTime FechaConsulta { get; set; }
    public int? MedicoID { get; set; }
    public string? NombreMedico { get; set; }
    public string? Especialidad { get; set; }
    public string Diagnostico { get; set; } = string.Empty;
    public string Indicaciones { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime FechaEmision { get; set; }
    public bool EsAnulada { get; set; }
    public DateTime? FechaAnulacion { get; set; }
    public string? MotivoAnulacion { get; set; }
    public long? RecetaAnteriorID { get; set; }
}
