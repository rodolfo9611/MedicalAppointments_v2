using MediatR;
using MedicalAppointments.RecetasMedicas.Api.Requests;
using MedicalAppointments.RecetasMedicas.Application.Commands.CreateRecetaMedica;
using MedicalAppointments.RecetasMedicas.Application.Common;
using MedicalAppointments.RecetasMedicas.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.RecetasMedicas.Api.Controllers;

/// <summary>
/// Emisión de recetas anidada bajo una cita concluida. NO existe POST /api/RecetasMedicas.
/// </summary>
[ApiController]
[Route("api/Citas")]
[Produces("application/json")]
public sealed class CitasRecetasMedicasController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>Inicializa el controlador con el mediador de MediatR.</summary>
    /// <param name="mediator">Mediador de la aplicación.</param>
    public CitasRecetasMedicasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Emite una receta para una cita concluida con al menos una atención registrada.
    /// El cuerpo solo contiene diagnóstico, indicaciones, tratamiento y observaciones opcional.
    /// </summary>
    /// <param name="citaId">Identificador de la cita concluida.</param>
    /// <param name="request">Datos clínicos de la receta.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpPost("{citaId:long}/RecetasMedicas")]
    [ProducesResponseType(typeof(ApplicationResult<RecetaMedicaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CrearReceta(
        long citaId,
        [FromBody] CreateRecetaMedicaRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new CreateRecetaMedicaCommand(
                citaId,
                request.Diagnostico,
                request.Indicaciones,
                request.Tratamiento,
                request.Observaciones),
            cancellationToken);

        if (!result.Success)
            return StatusCode(result.StatusCode, result);

        return CreatedAtAction(
            "GetById",
            "RecetasMedicas",
            new { id = result.Data!.RecetaMedicaID },
            result);
    }
}
