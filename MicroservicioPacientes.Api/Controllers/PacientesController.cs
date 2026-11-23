using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroservicioPacientes.Application.Commands.CreatePaciente;
using MicroservicioPacientes.Application.Commands.CreatePacientesRange;
using MicroservicioPacientes.Application.Commands.DeletePaciente;
using MicroservicioPacientes.Application.Commands.RegistrarEventoHistorial;
using MicroservicioPacientes.Application.Commands.UpdatePaciente;
using MicroservicioPacientes.Application.DTOs;
using MicroservicioPacientes.Application.Exceptions;
using MicroservicioPacientes.Application.Queries.GetHistorialMedico;
using MicroservicioPacientes.Application.Queries.GetPacienteById;
using MicroservicioPacientes.Application.Queries.GetPacienteOneBy;
using MicroservicioPacientes.Application.Queries.GetPacientes;
using MicroservicioPacientes.Application.Queries.GetPacientesPaged;
using MicroservicioPacientes.Application.Queries.GetSecuenciasAtencion;
using Nuget.Persistence.Common;

namespace MicroservicioPacientes.Api.Controllers;

/// <summary>
/// API de gestión de pacientes y de su historial clínico.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Consulta el listado completo de pacientes, incluyendo su historial clínico.
    /// </summary>
    /// <remarks>Equivale al antiguo uso de GetAllAsync del repositorio genérico.</remarks>
    /// <returns>Listado de pacientes.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GetPacienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var pacientes = await _mediator.Send(new GetPacientesQuery(), cancellationToken);
        return Ok(pacientes);
    }

    /// <summary>
    /// Consulta un paciente específico por su identificador.
    /// </summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El paciente solicitado o 404 si no existe.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetPacienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var paciente = await _mediator.Send(new GetPacienteByIdQuery(id), cancellationToken);
        if (paciente is null) return NotFound();
        return Ok(paciente);
    }

    /// <summary>
    /// Consulta pacientes con paginación, filtro dinámico y OrderBy dinámico por cualquier campo.
    /// </summary>
    /// <param name="pageNumber">Número de página (empezando en 1).</param>
    /// <param name="pageSize">Tamaño de página.</param>
    /// <param name="filter">Expresión de filtro dinámica, por ejemplo: CodigoPaciente == "PAC-000001".</param>
    /// <param name="orderBy">Campo o expresión de ordenamiento.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de pacientes.</returns>
    [HttpGet("paged")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        [FromQuery] string? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "pageNumber y pageSize deben ser mayores o iguales a 1." });

        try
        {
            var result = await _mediator.Send(
                new GetPacientesPagedQuery(pageNumber, pageSize, filter, orderBy),
                cancellationToken);

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Consulta un paciente con filtrado genérico tipo expresión (GetOneBy).
    /// </summary>
    /// <param name="filter">Expresión de filtro dinámica.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El paciente encontrado o 404 si no existe.</returns>
    [HttpGet("one")]
    [ProducesResponseType(typeof(GetPacienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOneBy([FromQuery] string filter, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return BadRequest(new HttpResponse<object> { Success = false, Message = "El filtro es obligatorio." });

        try
        {
            var paciente = await _mediator.Send(new GetPacienteOneByQuery(filter), cancellationToken);
            if (paciente is null) return NotFound();
            return Ok(paciente);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo paciente.
    /// </summary>
    /// <param name="paciente">Datos maestros del paciente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El paciente creado con su identificador.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GetPacienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] CreatePacienteDto paciente, CancellationToken cancellationToken)
    {
        try
        {
            var creado = await _mediator.Send(new CreatePacienteCommand(paciente), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = creado.PacienteID }, creado);
        }
        catch (DuplicateCodigoPacienteException ex)
        {
            return Conflict(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Crea un rango de pacientes a partir de una lista, con un único guardado (AddRange).
    /// </summary>
    /// <param name="pacientes">Lista de pacientes a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Cantidad de pacientes guardados.</returns>
    [HttpPost("range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PostRange(
        [FromBody] List<CreatePacienteDto> pacientes,
        CancellationToken cancellationToken)
    {
        if (pacientes is null || pacientes.Count == 0)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "La lista de pacientes no puede estar vacia." });

        try
        {
            var count = await _mediator.Send(new CreatePacientesRangeCommand(pacientes), cancellationToken);
            return Ok(new HttpResponse<int> { Success = true, Message = "Rango de pacientes guardado correctamente.", Data = count });
        }
        catch (DuplicateCodigoPacienteException ex)
        {
            return Conflict(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza los datos de un paciente existente.
    /// </summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <param name="paciente">Nuevos datos maestros del paciente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>204 si se actualizó, 404 si no existe o 409 si el código ya está en uso.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePacienteDto paciente, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new UpdatePacienteCommand(id, paciente), cancellationToken);
            return NoContent();
        }
        catch (PacienteNotFoundException)
        {
            return NotFound();
        }
        catch (DuplicateCodigoPacienteException ex)
        {
            return Conflict(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un paciente existente.
    /// </summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>204 si se eliminó, 404 si no existe.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var eliminado = await _mediator.Send(new DeletePacienteCommand(id), cancellationToken);
        if (!eliminado) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Consulta el historial clínico de un paciente con filtros y paginación.
    /// </summary>
    /// <param name="id">Identificador del paciente.</param>
    /// <param name="tipoEvento">Filtro opcional por tipo de evento.</param>
    /// <param name="fechaDesde">Filtro opcional de fecha de evento desde.</param>
    /// <param name="fechaHasta">Filtro opcional de fecha de evento hasta.</param>
    /// <param name="pageNumber">Número de página (empezando en 1).</param>
    /// <param name="pageSize">Tamaño de página.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de eventos del historial clínico.</returns>
    [HttpGet("{id:int}/historial")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistorial(
        int id,
        [FromQuery] string? tipoEvento = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "pageNumber y pageSize deben ser mayores o iguales a 1." });

        var historial = await _mediator.Send(
            new GetHistorialMedicoQuery(id, tipoEvento, fechaDesde, fechaHasta, pageNumber, pageSize),
            cancellationToken);

        if (historial is null) return NotFound();
        return Ok(historial);
    }

    /// <summary>
    /// Registra un evento en el historial clínico del paciente. Es idempotente por EventKey:
    /// si el evento ya existe no se duplica y la operación devuelve OK.
    /// </summary>
    /// <param name="id">Identificador del paciente de la ruta.</param>
    /// <param name="evento">Evento a registrar (mismo contrato usado por Citas).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>OK indicando si el evento fue creado o ya existía.</returns>
    [HttpPost("{id:int}/historial/eventos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegistrarEventoHistorial(
        int id,
        [FromBody] EventoHistorialRequestDto evento,
        CancellationToken cancellationToken)
    {
        if (evento.PacienteID != id)
        {
            return BadRequest(new HttpResponse<object>
            {
                Success = false,
                Message = "El PacienteID del cuerpo debe coincidir con el identificador de la ruta."
            });
        }

        try
        {
            var creado = await _mediator.Send(
                new RegistrarEventoHistorialCommand(id, evento),
                cancellationToken);

            return Ok(new HttpResponse<bool>
            {
                Success = true,
                Message = creado
                    ? "Evento registrado correctamente."
                    : "El evento ya estaba registrado; no se duplicó.",
                Data = creado
            });
        }
        catch (PacienteNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Devuelve las secuencias de eventos TipoEvento == "AtencionRegistrada" de una cita,
    /// ordenadas de menor a mayor. Contrato crítico consumido por el microservicio de Citas.
    /// </summary>
    /// <param name="citaId">Identificador de la cita.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Arreglo de secuencias de atención.</returns>
    [HttpGet("historial/cita/{citaId:long}/atenciones")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSecuenciasAtencion(long citaId, CancellationToken cancellationToken)
    {
        var secuencias = await _mediator.Send(new GetSecuenciasAtencionQuery(citaId), cancellationToken);
        return Ok(secuencias);
    }
}
