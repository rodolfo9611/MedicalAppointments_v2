namespace MedicalAppointments.RecetasMedicas.Domain.Entities;

/// <summary>
/// Receta médica emitida a partir de una cita concluida. Conserva un snapshot
/// de los datos maestros para que el historial sea inmutable en el tiempo.
/// </summary>
public class RecetaMedica
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
