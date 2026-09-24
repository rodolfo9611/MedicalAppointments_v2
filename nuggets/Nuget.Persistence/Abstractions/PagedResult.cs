namespace Nuget.Persistence.Abstractions;

public class PagedResult<TEntity>
{
    public IEnumerable<TEntity> Data { get; set; } = new List<TEntity>();
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage => (int)Math.Ceiling((double)TotalRecords / PageSize);
}