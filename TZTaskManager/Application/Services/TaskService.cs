using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TZTaskManager.Application.DTOs;
using TZTaskManager.Application.Interfaces;
using TZTaskManager.Domain.Entities;
using TZTaskManager.Domain.Interfaces;
using TZTaskManager.Infrastructure.Data;
using TaskEntity = TZTaskManager.Domain.Entities.Task;

namespace TZTaskManager.Application.Services
{
    /// <summary>
    /// Сервис для работы с задачами
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskService> _logger;
        private readonly ApplicationDbContext _context;

        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TaskService> logger,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default)
        {
            var tasks = await _context.Tasks
                .Include(t => t.TaskType)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async System.Threading.Tasks.Task<TaskDto?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskType)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (task == null)
            {
                return null;
            }

            return _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default)
        {
            var taskTypeExists = await _unitOfWork.TaskTypes.ExistsAsync(createTaskDto.TaskTypeId, cancellationToken);
            if (!taskTypeExists)
            {
                throw new InvalidOperationException($"Тип задачи с ID {createTaskDto.TaskTypeId} не найден");
            }

            var task = _mapper.Map<TaskEntity>(createTaskDto);
            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _context.Entry(task).Reference(t => t.TaskType).LoadAsync(cancellationToken);

            _logger.LogInformation("Создана задача с ID {TaskId}", task.Id);

            return _mapper.Map<TaskDto>(task);
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
            {
                throw new KeyNotFoundException($"Задача с ID {id} не найдена");
            }

            var taskTypeExists = await _unitOfWork.TaskTypes.ExistsAsync(updateTaskDto.TaskTypeId, cancellationToken);
            if (!taskTypeExists)
            {
                throw new InvalidOperationException($"Тип задачи с ID {updateTaskDto.TaskTypeId} не найден");
            }

            _mapper.Map(updateTaskDto, task);
            await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Обновлена задача с ID {TaskId}", id);
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
            if (task == null)
            {
                throw new KeyNotFoundException($"Задача с ID {id} не найдена");
            }

            await _unitOfWork.Tasks.DeleteAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Удалена задача с ID {TaskId}", id);
        }
    }
}

