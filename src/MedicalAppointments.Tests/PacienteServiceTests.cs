using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MedicalAppointments.Core.Citas.Configs;
using MedicalAppointments.Core.Citas.Exceptions;
using MedicalAppointments.Core.Citas.Services;
using MedicalAppointments.Persistence.Citas.Services;
using MedicalAppointments.Tests.Fakes;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Interfaces.IServices;
using Xunit;

namespace MedicalAppointments.Tests;

/// <summary>
/// Pruebas de PacienteService sobre un RestBuilder real con handler falso:
/// así se verifica también la serializacion real y la traduccion de ApiException.
/// </summary>
public class PacienteServiceTests
{
    private static IPacienteService CreateService(Func<HttpRequestMessage, HttpResponseMessage> factory)
    {
        var client = new HttpClient(new FakeHttpMessageHandler(factory));
        IRest rest = new RestBuilder(client, Options.Create(new RequestSettings { EnableRequestLogs = false }));
        var options = Options.Create(new PacientesOptions { BaseUrl = "http://localhost:5015" });

        return new PacienteService(rest, options);
    }

    [Fact]
    public async Task ObtenerPacienteAsync_ConsultaElEndpointDelMicroservicioDePacientes()
    {
        HttpRequestMessage? enviado = null;
        var service = CreateService(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":7,\"nombre\":\"Carlos\",\"apellido\":\"Perez\",\"documentoIdentidad\":\"12345678-9\",\"fechaNacimiento\":\"1990-05-04T00:00:00\"}")
            };
        });

        var paciente = await service.ObtenerPacienteAsync(7);

        Assert.NotNull(paciente);
        Assert.Equal(7, paciente!.Id);
        Assert.Equal("Carlos", paciente.Nombre);
        Assert.Equal("Perez", paciente.Apellido);
        Assert.Equal("12345678-9", paciente.DocumentoIdentidad);
        Assert.Equal("http://localhost:5015/api/pacientes/7", enviado!.RequestUri!.ToString());
        Assert.True(enviado.Headers.Contains("X-Correlation-Id"));
    }

    [Fact]
    public async Task ObtenerPacienteAsync_CuandoElPacienteNoExiste_DevuelveNull()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        var paciente = await service.ObtenerPacienteAsync(999);

        Assert.Null(paciente);
    }

    [Fact]
    public async Task ObtenerPacienteAsync_CuandoElMicroservicioEstaCaido_LanzaPacienteNoDisponibleException()
    {
        var service = CreateService(_ => throw new HttpRequestException("Connection refused"));

        var exception = await Assert.ThrowsAsync<PacienteNoDisponibleException>(() => service.ObtenerPacienteAsync(1));

        Assert.Equal("Network", exception.Motivo);
        Assert.Null(exception.CodigoHttp);
    }

    [Fact]
    public async Task ObtenerPacienteAsync_CuandoElMicroservicioRespondeError_LanzaExcepcionConElCodigoHttp()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{\"error\":\"boom\"}")
        });

        var exception = await Assert.ThrowsAsync<PacienteNoDisponibleException>(() => service.ObtenerPacienteAsync(1));

        Assert.Equal("HttpError", exception.Motivo);
        Assert.Equal(500, exception.CodigoHttp);
        Assert.Contains("boom", exception.Respuesta);
    }
}
