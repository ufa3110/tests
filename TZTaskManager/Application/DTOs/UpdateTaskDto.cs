using TZTaskManager.Domain.Enums;
using TaskStatusEnum = TZTaskManager.Domain.Enums.TaskStatus;

namespace TZTaskManager.Application.DTOs
{
    /// <summary>
    /// DTO для обновления задачи
    /// </summary>
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TaskTypeId { get; set; }
        public TaskStatusEnum Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

