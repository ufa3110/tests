using TZTaskManager.Application.DTOs;

namespace TZTaskManager.Application.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с задачами
    /// </summary>
    public interface ITaskService
    {
        System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<TaskDto?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default);
        System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
    }
}

