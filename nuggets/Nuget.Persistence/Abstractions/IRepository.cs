using System.Linq.Expressions;

namespace Nuget.Persistence.Abstractions;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> GetByIdAsync(TKey id);

    Task<TEntity?> GetOneByAsync(
        Expression<Func<TEntity, bool>> filter,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TKey> AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);

    Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        string? orderBy = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByExpression = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);
}