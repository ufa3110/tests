namespace TZTaskManager.Domain.Entities
{
    /// <summary>
    /// Тип задачи - категория задач
    /// </summary>
    public class TaskType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}

