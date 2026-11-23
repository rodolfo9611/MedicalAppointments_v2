using MediatR;

namespace MicroservicioPacientes.Application.Queries.GetSecuenciasAtencion;

/// <summary>
/// Consulta las secuencias de atención registradas para una cita.
/// Contrato usado por el microservicio de Citas para calcular la siguiente secuencia.
/// </summary>
public sealed record GetSecuenciasAtencionQuery(long CitaId) : IRequest<int[]>;

/// <summary>Manejador de <see cref="GetSecuenciasAtencionQuery"/>.</summary>
public sealed class GetSecuenciasAtencionQueryHandler : IRequestHandler<GetSecuenciasAtencionQuery, int[]>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetSecuenciasAtencionQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<int[]> Handle(GetSecuenciasAtencionQuery request, CancellationToken cancellationToken)
    {
        return await _pacienteRepository.GetSecuenciasAtencionAsync(request.CitaId, cancellationToken);
    }
}
