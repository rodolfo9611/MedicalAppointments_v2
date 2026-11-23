namespace MedicalAppointments.RecetasMedicas.Application.Common;

/// <summary>
/// Resultado de un comando con código HTTP sugerido y advertencias de
/// integraciones externas (la operación principal ya quedó persistida).
/// </summary>
public sealed class ApplicationResult<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; } = 200;
    public string? Message { get; init; }
    public T? Data { get; init; }
    public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();

    public static ApplicationResult<T> Ok(
        T data,
        int statusCode = 200,
        string? message = null,
        IReadOnlyList<string>? warnings = null)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Warnings = warnings ?? Array.Empty<string>()
        };

    public static ApplicationResult<T> Fail(
        int statusCode,
        string message,
        T? data = default,
        IReadOnlyList<string>? warnings = null)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Warnings = warnings ?? Array.Empty<string>()
        };
}
