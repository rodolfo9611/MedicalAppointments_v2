using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nuget.Persistence.Abstractions;

namespace Nuget.Persistence.Implementations;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    protected virtual IQueryable<TEntity> Query(bool asNoTracking = true)
    {
        return asNoTracking ? DbSet.AsNoTracking() : DbSet.AsQueryable();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TKey> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();

        var keyName = Context.Model
            .FindEntityType(typeof(TEntity))!
            .FindPrimaryKey()!
            .Properties[0].Name;

        var keyValue = Context.Entry(entity).Property(keyName).CurrentValue;

        return (TKey)keyValue!;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TEntity entity)
    {
        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "pageNumber must be 1 or greater.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be 1 or greater.");

        var query = ApplyIncludes(Query(asNoTracking), includes);
        if (filter != null) query = query.Where(filter);
        if (splitQuery) query = query.AsSplitQuery();

        // Count against the filtered (but not yet ordered/paged) query — computed
        // with CountAsync so it doesn't block a thread pool thread synchronously.
        int totalRecords = await query.CountAsync(cancellationToken);

        query = orderBy != null ? orderBy(query) : query;
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var data = await query.ToListAsync(cancellationToken);

        return new PagedResult<TEntity>
        {
            Data = data,
            TotalRecords = totalRecords,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }

    private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, Expression<Func<TEntity, object>>[] includes)
    {
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return query;
    }
}
