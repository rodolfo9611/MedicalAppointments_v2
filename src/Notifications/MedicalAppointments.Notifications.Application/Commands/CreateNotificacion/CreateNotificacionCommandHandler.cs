using MediatR;
using MedicalAppointments.Notifications.Application.DTOs;
using MedicalAppointments.Notifications.Domain.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Commands.CreateNotificacion;

public sealed class CreateNotificacionCommandHandler
    : IRequestHandler<CreateNotificacionCommand, HttpResponse<GetNotificacionDto>>
{
    private readonly INotificacionRepository _repository;

    public CreateNotificacionCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<HttpResponse<GetNotificacionDto>> Handle(
        CreateNotificacionCommand request,
        CancellationToken cancellationToken)
    {
        var entidad = request.Notificacion.ToEntity();
        await _repository.AddAsync(entidad);

        return new HttpResponse<GetNotificacionDto>
        {
            Success = true,
            Message = "Notificación creada correctamente.",
            Data = entidad.ToDto()
        };
    }
}
