using MedicalAppointments.Domain.Citas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadosCitaController : ControllerBase
{
    private readonly IEstadoCitaRepository _estadoCitaRepository;

    public EstadosCitaController(IEstadoCitaRepository estadoCitaRepository)
    {
        _estadoCitaRepository = estadoCitaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var estados = await _estadoCitaRepository.GetAllAsync();
        return Ok(estados);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var estado = await _estadoCitaRepository.GetByIdAsync(id);
        if (estado is null)
            return NotFound();

        return Ok(estado);
    }
}
