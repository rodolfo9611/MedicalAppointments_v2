using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Enums;

namespace MedicalAppointments.Core.Citas.DTOs;

public static class CitaMappingExtensions
{
    public static GetCitaDto ToDto(this Cita cita)
    {
        var estado = Enum.IsDefined(typeof(EstadoCitaEnum), cita.EstadoCitaID)
            ? (EstadoCitaEnum)cita.EstadoCitaID
            : EstadoCitaEnum.Agendada;

        return new GetCitaDto
        {
            CitaID = cita.CitaID,
            PacienteID = cita.PacienteID,
            MedicoID = cita.MedicoID,
            EspecialidadID = cita.EspecialidadID,
            ConsultorioID = cita.ConsultorioID,
            EstadoCitaID = cita.EstadoCitaID,
            Estado = estado,
            EstadoNombre = estado.ToString(),
            FechaHoraInicio = cita.FechaHoraInicio,
            FechaHoraFin = cita.FechaHoraFin,
            Motivo = cita.Motivo,
            Observaciones = cita.Observaciones,
            NombreMedico = cita.NombreMedico,
            Especialidad = cita.Especialidad,
            UsuarioCreacionID = cita.UsuarioCreacionID,
            FechaCreacion = cita.FechaCreacion
        };
    }
}
