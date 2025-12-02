using TZTaskManager.Domain.Enums;
using TaskStatusEnum = TZTaskManager.Domain.Enums.TaskStatus;

namespace TZTaskManager.Application.DTOs
{
    /// <summary>
    /// DTO для отображения задачи
    /// </summary>
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TaskTypeId { get; set; }
        public string TaskTypeName { get; set; } = string.Empty;
        public TaskStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}

