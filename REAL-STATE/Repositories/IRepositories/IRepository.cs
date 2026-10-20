using System.Linq.Expressions;

namespace REAL_STATE.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool tracked = false);

        Task<T?> GetAsync(
            Expression<Func<T, bool>>? filter,
            string? includeProperties = null,
            bool tracked = false);

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        void Remove(T entity);

        Task Save();
    }
}
