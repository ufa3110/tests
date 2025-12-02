using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using TZTaskManager.Domain.Entities;
using TZTaskManager.Domain.Interfaces;
using TaskEntity = TZTaskManager.Domain.Entities.Task;

namespace TZTaskManager.Infrastructure.Data
{
    /// <summary>
    /// Реализация Unit of Work
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private IRepository<TaskEntity>? _tasks;
        private IRepository<TaskType>? _taskTypes;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<TaskEntity> Tasks
        {
            get
            {
                _tasks ??= new Repository<TaskEntity>(_context);
                return _tasks;
            }
        }

        public IRepository<TaskType> TaskTypes
        {
            get
            {
                _taskTypes ??= new Repository<TaskType>(_context);
                return _taskTypes;
            }
        }

        public async System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

