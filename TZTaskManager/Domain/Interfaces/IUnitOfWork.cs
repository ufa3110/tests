using TZTaskManager.Domain.Entities;
using TaskEntity = TZTaskManager.Domain.Entities.Task;

namespace TZTaskManager.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс Unit of Work для управления транзакциями
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TaskEntity> Tasks { get; }
        IRepository<TaskType> TaskTypes { get; }
        System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}

