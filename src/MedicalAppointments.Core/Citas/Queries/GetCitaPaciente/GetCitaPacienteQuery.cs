using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaPaciente;

/// <summary>
/// Devuelve null cuando la cita no existe o el paciente ya no existe en el microservicio de Pacientes.
/// Lanza ValidationException cuando la llamada al otro microservicio falla por motivos técnicos.
/// </summary>
public record GetCitaPacienteQuery(long CitaID) : IRequest<HttpResponse<GetPacienteExternoDto>?>;
