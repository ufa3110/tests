using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TZTaskManager.Domain.Entities;
using TZTaskManager.Domain.Interfaces;

namespace TZTaskManager.Infrastructure.Data
{
    /// <summary>
    /// Базовая реализация репозитория
    /// </summary>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async System.Threading.Tasks.Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async System.Threading.Tasks.Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async System.Threading.Tasks.Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async System.Threading.Tasks.Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual System.Threading.Tasks.Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public virtual System.Threading.Tasks.Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public virtual async System.Threading.Tasks.Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}

