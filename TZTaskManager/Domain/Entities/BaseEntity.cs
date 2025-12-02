namespace TZTaskManager.Domain.Entities
{
    /// <summary>
    /// Базовый класс для всех сущностей
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

