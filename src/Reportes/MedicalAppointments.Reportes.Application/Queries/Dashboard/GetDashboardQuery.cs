using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;

namespace MedicalAppointments.Reportes.Application.Queries.Dashboard;

/// <summary>
/// Consulta los indicadores agregados del dashboard.
/// </summary>
public sealed record GetDashboardQuery : IRequest<DashboardDto>;
