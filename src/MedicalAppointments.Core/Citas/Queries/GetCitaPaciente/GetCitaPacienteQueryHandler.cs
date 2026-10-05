using System.ComponentModel.DataAnnotations;
using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Core.Citas.Exceptions;
using MedicalAppointments.Core.Citas.Services;
using MedicalAppointments.Domain.Citas.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaPaciente;

public class GetCitaPacienteQueryHandler : IRequestHandler<GetCitaPacienteQuery, HttpResponse<GetPacienteExternoDto>?>
{
    private readonly ICitaRepository _citaRepository;
    private readonly IPacienteService _pacienteService;

    public GetCitaPacienteQueryHandler(ICitaRepository citaRepository, IPacienteService pacienteService)
    {
        _citaRepository = citaRepository;
        _pacienteService = pacienteService;
    }

    public async Task<HttpResponse<GetPacienteExternoDto>?> Handle(GetCitaPacienteQuery request, CancellationToken cancellationToken)
    {
        var cita = await _citaRepository.GetByIdAsync(request.CitaID);
        if (cita is null)
            return null;

        GetPacienteExternoDto? paciente;
        try
        {
            paciente = await _pacienteService.ObtenerPacienteAsync(cita.PacienteID, cancellationToken);
        }
        catch (PacienteNoDisponibleException ex)
        {
            // Falla técnica del otro microservicio: se traduce a un 400 con el diagnóstico.
            throw new ValidationException(ex.Message);
        }

        if (paciente is null)
            return null;

        return new HttpResponse<GetPacienteExternoDto>
        {
            Success = true,
            Message = "Paciente consultado en el microservicio de Pacientes.",
            Data = paciente
        };
    }
}
