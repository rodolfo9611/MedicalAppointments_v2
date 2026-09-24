namespace Nuget.Persistence.Common;

public class PagedDto<TEntity> where TEntity : class
{
    public int TotalRecords { get; set; }
    public int TotalPage { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public TEntity Data { get; set; } = default!;

    public PagedDto()
    {
    }

    public PagedDto(int totalRecords, int currentPage, int pageSize, TEntity data)
    {
        TotalRecords = totalRecords;
        TotalPage = pageSize > 0 ? (int)Math.Ceiling(totalRecords / (double)pageSize) : 0;
        CurrentPage = currentPage;
        PageSize = pageSize;
        Data = data;
    }
}