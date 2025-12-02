using System.Linq.Expressions;
using System.Threading.Tasks;
using TZTaskManager.Domain.Entities;

namespace TZTaskManager.Domain.Interfaces
{
    /// <summary>
    /// Базовый интерфейс репозитория
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        System.Threading.Tasks.Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    }
}

