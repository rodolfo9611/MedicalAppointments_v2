namespace MedicalAppointments.Reportes.Application.Common;

/// <summary>
/// Base común para las consultas paginadas de reportes.
/// </summary>
public abstract class PagedQueryBase
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>Número de página (mayor o igual que 1).</summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>Tamaño de página (entre 1 y 500).</summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : Math.Min(value, 500);
    }
}
