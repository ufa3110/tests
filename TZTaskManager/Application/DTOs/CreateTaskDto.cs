using TZTaskManager.Domain.Enums;
using TaskStatusEnum = TZTaskManager.Domain.Enums.TaskStatus;

namespace TZTaskManager.Application.DTOs
{
    /// <summary>
    /// DTO для создания задачи
    /// </summary>
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TaskTypeId { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
        public DateTime? DueDate { get; set; }
    }
}

