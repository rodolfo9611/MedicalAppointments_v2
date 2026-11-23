using MediatR;
using MedicalAppointments.Core.Citas.DTOs;

namespace MedicalAppointments.Core.Citas.Queries.GetCitas;

/// <summary>
/// Consulta el listado completo del proceso de citas como DTOs enriquecidos.
/// </summary>
public record GetCitasQuery : IRequest<List<GetCitaDto>>;
