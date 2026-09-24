using System.ComponentModel.DataAnnotations;
using MediatR;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Domain.Citas.Entities;
using MedicalAppointments.Domain.Citas.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Core.Citas.Queries.GetCitaOneBy;

public class GetCitaOneByQueryHandler : IRequestHandler<GetCitaOneByQuery, HttpResponse<GetCitaDto?>>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitaOneByQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<HttpResponse<GetCitaDto?>> Handle(GetCitaOneByQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Filter))
                return new HttpResponse<GetCitaDto?> { Success = false, Message = "El filtro es obligatorio." };

            var filter = Filter.FromStringExpression<Cita>(request.Filter);
            var cita = await _citaRepository.GetOneByAsync(filter, cancellationToken: cancellationToken);

            if (cita is null)
                return new HttpResponse<GetCitaDto?> { Success = true, Message = "Sin resultados.", Data = null };

            return new HttpResponse<GetCitaDto?>
            {
                Success = true,
                Message = "Consulta realizada con filtrado generico.",
                Data = new GetCitaDto
                {
                    CitaID = cita.CitaID,
                    PacienteID = cita.PacienteID,
                    MedicoID = cita.MedicoID,
                    EspecialidadID = cita.EspecialidadID,
                    ConsultorioID = cita.ConsultorioID,
                    EstadoCitaID = cita.EstadoCitaID,
                    FechaHoraInicio = cita.FechaHoraInicio,
                    FechaHoraFin = cita.FechaHoraFin,
                    Motivo = cita.Motivo,
                    Observaciones = cita.Observaciones,
                    UsuarioCreacionID = cita.UsuarioCreacionID,
                    FechaCreacion = cita.FechaCreacion
                }
            };
        }
        catch (ValidationException ex)
        {
            return new HttpResponse<GetCitaDto?> { Success = false, Message = ex.Message };
        }
    }
}