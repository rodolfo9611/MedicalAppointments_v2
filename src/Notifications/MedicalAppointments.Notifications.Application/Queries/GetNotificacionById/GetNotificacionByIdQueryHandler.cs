using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using MedicalAppointments.Notifications.Domain.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Queries.GetNotificacionById;

public sealed class GetNotificacionByIdQueryHandler
    : IRequestHandler<GetNotificacionByIdQuery, HttpResponse<GetNotificacionDto>>
{
    private readonly INotificacionRepository _repository;

    public GetNotificacionByIdQueryHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<HttpResponse<GetNotificacionDto>> Handle(
        GetNotificacionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entidad = await _repository.GetByIdAsync(request.Id);
        if (entidad is null)
        {
            return new HttpResponse<GetNotificacionDto>
            {
                Success = false,
                Message = $"No se encontró la notificación #{request.Id}."
            };
        }

        return new HttpResponse<GetNotificacionDto>
        {
            Success = true,
            Data = entidad.ToDto()
        };
    }
}
