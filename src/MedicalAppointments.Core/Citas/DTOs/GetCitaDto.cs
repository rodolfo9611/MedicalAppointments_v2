using MedicalAppointments.Domain.Citas.Enums;

namespace MedicalAppointments.Core.Citas.DTOs;

public class GetCitaDto
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public int? EspecialidadID { get; set; }
    public int? ConsultorioID { get; set; }
    public int EstadoCitaID { get; set; }
    public EstadoCitaEnum Estado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public DateTime FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public DateTime Fecha => FechaHoraInicio.Date;
    public TimeSpan Hora => FechaHoraInicio.TimeOfDay;
    public string? Motivo { get; set; }
    public string? Observaciones { get; set; }
    public string? NombreMedico { get; set; }
    public string? Especialidad { get; set; }
    public int? UsuarioCreacionID { get; set; }
    public DateTime FechaCreacion { get; set; }
}
