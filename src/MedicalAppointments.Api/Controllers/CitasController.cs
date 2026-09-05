using MediatR;
using MedicalAppointments.Core.Citas.Commands.CreateCita;
using MedicalAppointments.Core.Citas.Queries.GetCitaById;
using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly ICitaRepository _citaRepository;
    private readonly IMediator _mediator;

    public CitasController(ICitaRepository citaRepository, IMediator mediator)
    {
        _citaRepository = citaRepository;
        _mediator = mediator;
    }

    /// <summary>Consulta el listado completo de citas.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var citas = await _citaRepository.GetAllAsync();
        return Ok(citas);
    }

    /// <summary>Consulta una cita especifica por su identificador.</summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var cita = await _mediator.Send(new GetCitaByIdQuery(id));
        if (cita is null)
            return NotFound();

        return Ok(cita);
    }

    /// <summary>Crea una nueva cita.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitaCommand command)
    {
        var newId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { CitaID = newId });
    }

    /// <summary>Actualiza los datos de una cita existente.</summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] Cita cita)
    {
        var existente = await _citaRepository.GetByIdAsync(id);
        if (existente is null)
            return NotFound();

        cita.CitaID = id;
        await _citaRepository.UpdateAsync(cita);
        return NoContent();
    }

    /// <summary>Elimina una cita existente.</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var existente = await _citaRepository.GetByIdAsync(id);
        if (existente is null)
            return NotFound();

        await _citaRepository.DeleteAsync(existente);
        return NoContent();
    }
}