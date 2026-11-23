using MediatR;
using MedicalAppointments.Notifications.Domain.Interfaces;
using Nuget.Persistence.Common;

namespace MedicalAppointments.Notifications.Application.Commands.DeleteNotificacion;

public sealed class DeleteNotificacionCommandHandler : IRequestHandler<DeleteNotificacionCommand, HttpResponse<bool>>
{
    private readonly INotificacionRepository _repository;

    public DeleteNotificacionCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<HttpResponse<bool>> Handle(
        DeleteNotificacionCommand request,
        CancellationToken cancellationToken)
    {
        var entidad = await _repository.GetByIdAsync(request.Id);
        if (entidad is null)
        {
            return new HttpResponse<bool>
            {
                Success = false,
                Message = $"No se encontró la notificación #{request.Id}.",
                Data = false
            };
        }

        await _repository.DeleteAsync(entidad);

        return new HttpResponse<bool>
        {
            Success = true,
            Message = "Notificación eliminada correctamente.",
            Data = true
        };
    }
}
