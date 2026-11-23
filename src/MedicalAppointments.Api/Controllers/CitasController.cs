using MediatR;
using MedicalAppointments.Core.Citas.Commands.CreateCita;
using MedicalAppointments.Core.Citas.Commands.CreateCitas;
using MedicalAppointments.Core.Citas.Commands.DeleteCita;
using MedicalAppointments.Core.Citas.Commands.UpdateCita;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Core.Citas.Queries.GetCitaById;
using MedicalAppointments.Core.Citas.Queries.GetCitaOneBy;
using MedicalAppointments.Core.Citas.Queries.GetCitas;
using MedicalAppointments.Core.Citas.Queries.GetCitasPaged;
using Microsoft.AspNetCore.Mvc;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Consulta el listado completo de citas como DTOs con estado legible y datos maestros.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var citas = await _mediator.Send(new GetCitasQuery());
        return Ok(citas);
    }

    /// <summary>Consulta una cita específica por su identificador.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var cita = await _mediator.Send(new GetCitaByIdQuery(id));
        if (cita is null)
            return NotFound();

        return Ok(cita);
    }

    /// <summary>Consulta citas con paginación genérica, filtro dinámico y OrderBy dinámico.</summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] GetCitasPagedQuery query)
    {
        if (query.PageNumber < 1 || query.PageSize < 1)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "pageNumber y pageSize deben ser mayores o iguales a 1." });

        var result = await _mediator.Send(query);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Consulta una cita con filtrado genérico tipo expresión (GetOneBy).</summary>
    [HttpGet("one")]
    public async Task<IActionResult> GetOneBy([FromQuery] string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return BadRequest(new HttpResponse<object> { Success = false, Message = "El filtro es obligatorio." });

        var result = await _mediator.Send(new GetCitaOneByQuery(filter));
        if (!result.Success)
            return BadRequest(result);

        if (result.Data is null)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Abre un nuevo proceso médico. Solo debe usarse cuando exista un motivo de consulta independiente;
    /// reprogramaciones, revisiones, cirugías y recetas evolucionan el mismo CitaID mediante PUT.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitaCommand command)
    {
        try
        {
            var newId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { CitaID = newId });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>Crea un rango de citas independientes con un único guardado (AddRange).</summary>
    [HttpPost("range")]
    public async Task<IActionResult> CreateRange([FromBody] CreateCitasCommand command)
    {
        if (command.Citas is null || command.Citas.Count == 0)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "La lista de citas no puede estar vacía." });

        try
        {
            var total = await _mediator.Send(command);
            return Ok(new HttpResponse<int> { Success = true, Message = "Rango de citas guardado correctamente.", Data = total });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>
    /// Evoluciona el mismo proceso médico: registra atención secuencial, reprograma dentro de
    /// PendienteResultados y cambia al estado clínico correspondiente sin crear otra cita.
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCitaDto datos)
    {
        if (id <= 0)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "El CitaID debe ser mayor a 0." });

        try
        {
            var cita = await _mediator.Send(new UpdateCitaCommand(id, datos));
            if (cita is null)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>Cancela lógicamente una cita conservando su CitaID y su historial clínico.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        if (id <= 0)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "El CitaID debe ser mayor a 0." });

        try
        {
            var eliminada = await _mediator.Send(new DeleteCitaCommand(id));
            if (!eliminada)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }
}
