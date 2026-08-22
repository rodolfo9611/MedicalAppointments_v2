using Microsoft.AspNetCore.Mvc;
using MicroservicioPacientes.Application;
using MicroservicioPacientes.Domain;

namespace MicroservicioPacientes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteRepository _pacienteRepository;

    public PacientesController(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pacientes = await _pacienteRepository.ObtenerTodosAsync();
        return Ok(pacientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var paciente = await _pacienteRepository.ObtenerPorIdAsync(id);
        if (paciente == null) return NotFound();
        return Ok(paciente);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Paciente paciente)
    {
        await _pacienteRepository.AgregarAsync(paciente);
        return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
    }
}