using MediatR;
using MedicalAppointments.Core.Citas.Commands.CreateCita;
using MedicalAppointments.Core.Citas.Queries.GetCitaById;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var citas = await _citaRepository.GetAllAsync();
        return Ok(citas);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var cita = await _mediator.Send(new GetCitaByIdQuery(id));
        if (cita is null)
            return NotFound();

        return Ok(cita);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitaCommand command)
    {
        var newId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { CitaID = newId });
    }
}