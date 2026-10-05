using MedicalAppointments.Core.Citas.Configs;
using MedicalAppointments.Core.Citas.DTOs;
using MedicalAppointments.Core.Citas.Exceptions;
using MedicalAppointments.Core.Citas.Services;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Interfaces.IServices;

namespace MedicalAppointments.Persistence.Citas.Services;

/// <summary>
/// Implementa el acceso al microservicio de Pacientes usando la librería NugetPackage_Rest.
/// Es la única capa que conoce IRest: traduce ApiException a PacienteNoDisponibleException.
/// </summary>
internal class PacienteService : IPacienteService
{
    private readonly IRest _rest;
    private readonly PacientesOptions _options;

    public PacienteService(IRest rest, IOptions<PacientesOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    public async Task<GetPacienteExternoDto?> ObtenerPacienteAsync(long pacienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _rest.Get
                .WithoutAuth()
                .WithHeaders(new Dictionary<string, string> { ["X-Correlation-Id"] = Guid.NewGuid().ToString() })
                .WithUri(_options.BaseUrl, $"/api/pacientes/{pacienteId}")
                .DeserializeWithAsync<GetPacienteExternoDto>();
        }
        catch (ApiException ex) when (ex.Reason == ApiFailureReason.HttpError && ex.StatusCode == 404)
        {
            // El paciente no existe en Pacientes: no es una falla técnica.
            return null;
        }
        catch (ApiException ex)
        {
            throw new PacienteNoDisponibleException(
                ex.Reason.ToString(),
                ex.StatusCode,
                ex.ResponseBody,
                $"No fue posible consultar el microservicio de Pacientes. Motivo: {ex.Reason}. " +
                $"Codigo HTTP: {ex.StatusCode?.ToString() ?? "sin respuesta"}. Detalle: {ex.ResponseBody ?? "sin cuerpo"}");
        }
    }
}
