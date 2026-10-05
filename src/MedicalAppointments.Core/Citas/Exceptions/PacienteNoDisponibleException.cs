namespace MedicalAppointments.Core.Citas.Exceptions;

/// <summary>
/// Error de negocio que traduce una falla técnica del microservicio de Pacientes.
/// La capa Core conoce esta excepción, no la ApiException de la librería HTTP:
/// así el dominio no depende de detalles técnicos.
/// </summary>
public class PacienteNoDisponibleException : Exception
{
    public string Motivo { get; }
    public int? CodigoHttp { get; }
    public string? Respuesta { get; }

    public PacienteNoDisponibleException(string motivo, int? codigoHttp, string? respuesta, string message)
        : base(message)
    {
        Motivo = motivo;
        CodigoHttp = codigoHttp;
        Respuesta = respuesta;
    }
}
