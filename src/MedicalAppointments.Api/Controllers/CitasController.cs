using MedicalAppointments.Domain.Citas.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly ICitaRepository _citaRepository;

    public CitasController(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
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
        var cita = await _citaRepository.GetByIdAsync(id);
        if (cita is null)
            return NotFound();

        return Ok(cita);
    }
}
