using MediatR;
using MedicalAppointments.RecetasMedicas.Api.Requests;
using MedicalAppointments.RecetasMedicas.Application.Commands.AnularRecetaMedica;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetHistorialByPaciente;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaActivaByCita;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaById;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetaImprimible;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetasAnuladas;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetasPagedReport;
using MedicalAppointments.RecetasMedicas.Application.Queries.GetRecetasVigentes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nuget.Persistence.Abstractions;

namespace MedicalAppointments.RecetasMedicas.Api.Controllers;

/// <summary>
/// Consultas, historial, impresión y anulación lógica de recetas médicas.
/// La creación SOLO existe en POST /api/Citas/{citaId}/RecetasMedicas.
/// </summary>
[ApiController]
[Route("api/RecetasMedicas")]
[Produces("application/json")]
public sealed class RecetasMedicasController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Inicializa el controlador con el mediador de MediatR.</summary>
    /// <param name="mediator">Mediador de la aplicación.</param>
    public RecetasMedicasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Devuelve las recetas vigentes (no anuladas) paginadas, ordenadas por fecha de emisión.
    /// </summary>
    /// <param name="pageNumber">Número de página (1 o mayor).</param>
    /// <param name="pageSize">Tamaño de página (1 o mayor).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RecetaMedicaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetVigentes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new { Message = "pageNumber y pageSize deben ser mayores o iguales a 1." });

        var result = await _mediator.Send(new GetRecetasVigentesQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>Devuelve el detalle de una receta, incluso si está anulada.</summary>
    /// <param name="id">Identificador de la receta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RecetaMedicaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var receta = await _mediator.Send(new GetRecetaByIdQuery(id), cancellationToken);
        return receta is null ? NotFound() : Ok(receta);
    }

    /// <summary>
    /// Devuelve la receta activa de una cita (EsAnulada == false) o 404 si no existe.
    /// Útil para mostrar/ocultar Emitir, Ver e Imprimir Receta en el frontend.
    /// </summary>
    /// <param name="citaId">Identificador de la cita.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("cita/{citaId:long}")]
    [ProducesResponseType(typeof(RecetaMedicaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActivaByCita(long citaId, CancellationToken cancellationToken = default)
    {
        var receta = await _mediator.Send(new GetRecetaActivaByCitaQuery(citaId), cancellationToken);
        if (receta is null)
        {
            return NotFound(new
            {
                Message = $"No existe receta activa para la cita #{citaId}.",
                CitaID = citaId
            });
        }

        return Ok(receta);
    }

    /// <summary>
    /// Devuelve el historial permanente de recetas de un paciente, incluyendo las anuladas.
    /// </summary>
    /// <param name="pacienteId">Identificador del paciente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("historial/paciente/{pacienteId:long}")]
    [ProducesResponseType(typeof(IReadOnlyList<RecetaMedicaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorialPaciente(
        long pacienteId,
        CancellationToken cancellationToken = default)
    {
        var historial = await _mediator.Send(new GetHistorialByPacienteQuery(pacienteId), cancellationToken);
        return Ok(historial);
    }

    /// <summary>Devuelve el listado de recetas anuladas lógicamente.</summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("anuladas")]
    [ProducesResponseType(typeof(IReadOnlyList<RecetaMedicaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnuladas(CancellationToken cancellationToken = default)
    {
        var anuladas = await _mediator.Send(new GetRecetasAnuladasQuery(), cancellationToken);
        return Ok(anuladas);
    }

    /// <summary>
    /// Consulta paginada y filtrada para Reportes: paciente, diagnóstico, rango de fechas
    /// y opción de incluir recetas anuladas.
    /// </summary>
    /// <param name="query">Filtros y paginación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResult<RecetaMedicaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] GetRecetasPagedReportQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.PageNumber < 1 || query.PageSize < 1)
            return BadRequest(new { Message = "pageNumber y pageSize deben ser mayores o iguales a 1." });

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Devuelve la receta en HTML imprimible (@media print) con datos del paciente,
    /// cita, médico, especialidad, diagnóstico, indicaciones, tratamiento,
    /// observaciones y fecha de emisión.
    /// </summary>
    /// <param name="id">Identificador de la receta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet("{id:long}/imprimible")]
    [Produces("text/html")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImprimible(long id, CancellationToken cancellationToken = default)
    {
        var html = await _mediator.Send(new GetRecetaImprimibleQuery(id), cancellationToken);
        if (html is null)
            return NotFound();

        return Content(html, "text/html");
    }

    /// <summary>
    /// Anula lógicamente una receta. El motivo puede enviarse en el query 'motivo'
    /// o en el cuerpo JSON { "motivo": "..." }. No elimina el registro.
    /// </summary>
    /// <param name="id">Identificador de la receta.</param>
    /// <param name="motivo">Motivo de la anulación (query string).</param>
    /// <param name="body">Cuerpo opcional con el motivo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(ApplicationResult<RecetaMedicaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Anular(
        long id,
        [FromQuery] string? motivo,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] AnularRecetaRequest? body,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new AnularRecetaMedicaCommand(id, motivo ?? body?.Motivo),
            cancellationToken);

        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
}
