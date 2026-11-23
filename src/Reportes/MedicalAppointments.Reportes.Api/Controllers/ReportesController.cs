using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Application.Queries.Citas;
using MedicalAppointments.Reportes.Application.Queries.Notificaciones;
using MedicalAppointments.Reportes.Application.Queries.Pacientes;
using MedicalAppointments.Reportes.Application.Queries.Recetas;
using Microsoft.AspNetCore.Mvc;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Reportes.Api.Controllers;

/// <summary>
/// Endpoints de solo lectura para los reportes de pacientes, citas, recetas y notificaciones.
/// </summary>
[ApiController]
[Route("api/reportes")]
public sealed class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Inicializa el controlador con el mediator de MediatR.</summary>
    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Reporte paginado de pacientes por documento, nombre y fecha de nacimiento.</summary>
    [HttpGet("pacientes")]
    [ProducesResponseType(typeof(PagedDto<List<PacienteReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedDto<List<PacienteReportDto>>>> GetPacientes(
        [FromQuery] GetReportePacientesQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>Reporte paginado de citas por estado, paciente y rango de fechas.</summary>
    [HttpGet("citas")]
    [ProducesResponseType(typeof(PagedDto<List<CitaReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedDto<List<CitaReportDto>>>> GetCitas(
        [FromQuery] GetReporteCitasQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>Reporte paginado de recetas vigentes (anuladas excluidas).</summary>
    [HttpGet("recetas")]
    [ProducesResponseType(typeof(PagedDto<List<RecetaReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedDto<List<RecetaReportDto>>>> GetRecetas(
        [FromQuery] GetReporteRecetasQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    /// <summary>Reporte paginado de notificaciones por tipo, estado, canal y rango de fechas.</summary>
    [HttpGet("notificaciones")]
    [ProducesResponseType(typeof(PagedDto<List<NotificacionReportDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedDto<List<NotificacionReportDto>>>> GetNotificaciones(
        [FromQuery] GetReporteNotificacionesQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(query, cancellationToken));
    }
}
