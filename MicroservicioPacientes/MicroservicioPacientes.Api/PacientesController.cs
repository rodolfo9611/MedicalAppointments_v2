using MediatR;
using Microsoft.AspNetCore.Mvc;
using MicroservicioPacientes.Application.Pacientes.Commands;
using MicroservicioPacientes.Application.Pacientes.Queries;

namespace MicroservicioPacientes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pacientes = await _mediator.Send(new ObtenerPacientesQuery());
        return Ok(pacientes);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CrearPacienteCommand command)
    {
        var pacienteId = await _mediator.Send(command);
        return Ok(new { Id = pacienteId, Mensaje = "Paciente creado exitosamente" });
    }
}