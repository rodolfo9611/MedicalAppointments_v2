using MedicalAppointments.Reportes.Domain.Common;

namespace MedicalAppointments.Reportes.Application.DTOs;

/// <summary>Registro de cita devuelto por el reporte de citas.</summary>
public sealed class CitaReportDto
{
    /// <summary>Identificador de la cita.</summary>
    public long CitaID { get; set; }

    /// <summary>Identificador del paciente.</summary>
    public long PacienteID { get; set; }

    /// <summary>Identificador del estado de la cita.</summary>
    public int EstadoCitaID { get; set; }

    /// <summary>Estado de la cita.</summary>
    public EstadoCitaRead Estado { get; set; }

    /// <summary>Fecha y hora de inicio de la cita.</summary>
    public DateTime FechaHoraInicio { get; set; }

    /// <summary>Nombre del médico.</summary>
    public string? NombreMedico { get; set; }

    /// <summary>Especialidad de la cita.</summary>
    public string? Especialidad { get; set; }
}
