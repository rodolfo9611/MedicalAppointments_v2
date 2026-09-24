using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MicroservicioPacientes.Application;
using MicroservicioPacientes.Domain;
using Nuget.Persistence.Common;

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

    /// <summary>Consulta el listado completo de pacientes (metodo generico GetAllAsync).</summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pacientes = await _pacienteRepository.GetAllAsync(asNoTracking: true, p => p.Historiales);
        return Ok(pacientes);
    }

    /// <summary>Consulta un paciente especifico por su identificador.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(id);
        if (paciente == null) return NotFound();
        return Ok(paciente);
    }

    /// <summary>Consulta pacientes con paginacion generica, filtro dinamico y OrderBy dinamico por cualquier campo.</summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filter = null,
        [FromQuery] string? orderBy = null)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "pageNumber y pageSize deben ser mayores o iguales a 1." });

        try
        {
            var result = await _pacienteRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                string.IsNullOrWhiteSpace(filter) ? null : Filter.FromStringExpression<Paciente>(filter),
                orderBy: orderBy,
                asNoTracking: true,
                includes: p => p.Historiales);

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>Consulta un paciente con filtrado generico tipo expresion (GetOneBy).</summary>
    [HttpGet("one")]
    public async Task<IActionResult> GetOneBy([FromQuery] string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return BadRequest(new HttpResponse<object> { Success = false, Message = "El filtro es obligatorio." });

        try
        {
            var paciente = await _pacienteRepository.GetOneByAsync(
                Filter.FromStringExpression<Paciente>(filter),
                includes: p => p.Historiales);

            if (paciente == null) return NotFound();
            return Ok(paciente);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new HttpResponse<object> { Success = false, Message = ex.Message });
        }
    }

    /// <summary>Crea un nuevo paciente.</summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Paciente paciente)
    {
        var id = await _pacienteRepository.AddAsync(paciente);
        return CreatedAtAction(nameof(GetById), new { id }, paciente);
    }

    /// <summary>Crea un rango de pacientes a partir de una lista, con un unico guardado (AddRange).</summary>
    [HttpPost("range")]
    public async Task<IActionResult> PostRange([FromBody] List<Paciente> pacientes)
    {
        if (pacientes == null || pacientes.Count == 0)
            return BadRequest(new HttpResponse<object> { Success = false, Message = "La lista de pacientes no puede estar vacia." });

        await _pacienteRepository.AddRangeAsync(pacientes);
        return Ok(new HttpResponse<int> { Success = true, Message = "Rango de pacientes guardado correctamente.", Data = pacientes.Count });
    }

    /// <summary>Actualiza los datos de un paciente existente.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Paciente paciente)
    {
        var existente = await _pacienteRepository.GetByIdAsync(id);
        if (existente == null) return NotFound();

        paciente.Id = id;
        await _pacienteRepository.UpdateAsync(paciente);
        return NoContent();
    }

    /// <summary>Elimina un paciente existente.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existente = await _pacienteRepository.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _pacienteRepository.DeleteAsync(existente);
        return NoContent();
    }
}