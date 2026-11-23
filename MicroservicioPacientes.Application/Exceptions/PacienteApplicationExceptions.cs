namespace MicroservicioPacientes.Application.Exceptions;

/// <summary>
/// Se lanza cuando el código de paciente ya está registrado.
/// </summary>
public sealed class DuplicateCodigoPacienteException : Exception
{
    public DuplicateCodigoPacienteException(string codigo)
        : base($"Ya existe un paciente con el código '{codigo}'.")
    {
        Codigo = codigo;
    }

    public string Codigo { get; }
}

/// <summary>
/// Se lanza cuando el paciente solicitado no existe.
/// </summary>
public sealed class PacienteNotFoundException : Exception
{
    public PacienteNotFoundException(int pacienteId)
        : base($"No se encontró el paciente con identificador {pacienteId}.")
    {
        PacienteId = pacienteId;
    }

    public int PacienteId { get; }
}
