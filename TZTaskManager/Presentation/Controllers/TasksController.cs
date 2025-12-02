using Microsoft.AspNetCore.Mvc;
using TZTaskManager.Application.DTOs;
using TZTaskManager.Application.Interfaces;

namespace TZTaskManager.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с задачами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все задачи
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(CancellationToken cancellationToken)
        {
            var tasks = await _taskService.GetAllTasksAsync(cancellationToken);
            return Ok(tasks);
        }

        /// <summary>
        /// Получить задачу по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> GetTask(int id, CancellationToken cancellationToken)
        {
            var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);
            if (task == null)
            {
                return NotFound(new { message = $"Задача с ID {id} не найдена" });
            }

            return Ok(task);
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto, CancellationToken cancellationToken)
        {
            var task = await _taskService.CreateTaskAsync(createTaskDto, cancellationToken);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Обновить задачу
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken)
        {
            await _taskService.UpdateTaskAsync(id, updateTaskDto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
        {
            await _taskService.DeleteTaskAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

