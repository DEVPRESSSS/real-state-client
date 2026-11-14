using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Utilities;

namespace REAL_STATE.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    private readonly DbSet<T> _entities;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        _entities = db.Set<T>();
    }

    public async Task AddAsync(T entity) => await _entities.AddAsync(entity);

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null, bool tracked = false)
        => await Query(filter, includeProperties, tracked).ToListAsync();

    public Task<T?> GetAsync(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool tracked = false)
        => Query(filter, includeProperties, tracked).FirstOrDefaultAsync();

    public Task<PagedResult<T>> GetPageAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        int page = 1, int pageSize = 20, Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        => orderBy(Query(filter, includeProperties, false)).ToPageAsync(page, pageSize);

    public void Remove(T entity) => _entities.Remove(entity);
    public async Task Save() => await _db.SaveChangesAsync();

    public Task UpdateAsync(T entity)
    {
        _entities.Update(entity);
        return Task.CompletedTask;
    }

    private IQueryable<T> Query(Expression<Func<T, bool>>? filter, string? includes, bool tracked)
    {
        IQueryable<T> query = tracked ? _entities : _entities.AsNoTracking();
        if (filter != null) query = query.Where(filter);
        if (!string.IsNullOrWhiteSpace(includes))
            foreach (var include in includes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                query = query.Include(include);
        return query;
    }
}
