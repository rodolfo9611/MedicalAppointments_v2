using MediatR;
using MedicalAppointments.Reportes.Application.DTOs;
using MedicalAppointments.Reportes.Application.Queries.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Reportes.Api.Controllers;

/// <summary>
/// Expone los indicadores agregados del negocio.
/// </summary>
[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Inicializa el controlador con el mediator de MediatR.</summary>
    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene el total de pacientes, conteos de citas por estado, recetas y notificaciones,
    /// además de los indicadores del día actual.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardQuery(), cancellationToken);
        return Ok(result);
    }
}
