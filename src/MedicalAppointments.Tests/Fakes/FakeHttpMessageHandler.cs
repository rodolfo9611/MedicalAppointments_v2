using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalAppointments.Tests.Fakes;

/// <summary>Sustituye la red: devuelve una respuesta fabricada o lanza la excepción indicada.</summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage>? _responseFactory;
    private readonly Exception? _exceptionToThrow;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public FakeHttpMessageHandler(Exception exceptionToThrow)
    {
        _exceptionToThrow = exceptionToThrow;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_exceptionToThrow != null)
            throw _exceptionToThrow;

        return Task.FromResult(_responseFactory!(request));
    }
}
