using TZTaskManager.Domain.Enums;
using TaskStatusEnum = TZTaskManager.Domain.Enums.TaskStatus;

namespace TZTaskManager.Domain.Entities
{
    /// <summary>
    /// Задача - основная сущность домена
    /// </summary>
    public class Task : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TaskTypeId { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;
        public DateTime? DueDate { get; set; }

        public TaskType TaskType { get; set; } = null!;
    }
}

