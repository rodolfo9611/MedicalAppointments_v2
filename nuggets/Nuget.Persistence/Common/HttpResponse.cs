namespace Nuget.Persistence.Common;

public class HttpResponse<T>
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public T? Data { get; set; }

    public HttpResponse()
    {
    }

    public HttpResponse(T data, string? message = null, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }
}
