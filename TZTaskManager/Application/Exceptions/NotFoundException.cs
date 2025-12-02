namespace TZTaskManager.Application.Exceptions
{
    /// <summary>
    /// Исключение для случаев, когда сущность не найдена
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Сущность \"{name}\" ({key}) не найдена.")
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}

