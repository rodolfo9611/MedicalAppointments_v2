using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using MedicalAppointments.Notifications.Domain.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Commands.UpdateNotificacion;

public sealed class UpdateNotificacionCommandHandler
    : IRequestHandler<UpdateNotificacionCommand, HttpResponse<GetNotificacionDto>>
{
    private readonly INotificacionRepository _repository;

    public UpdateNotificacionCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<HttpResponse<GetNotificacionDto>> Handle(
        UpdateNotificacionCommand request,
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

        request.Notificacion.ApplyTo(entidad);
        await _repository.UpdateAsync(entidad);

        return new HttpResponse<GetNotificacionDto>
        {
            Success = true,
            Message = "Notificación actualizada correctamente.",
            Data = entidad.ToDto()
        };
    }
}
