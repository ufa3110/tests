namespace TrueCodeTestSolution.TestClass
{
    /// <summary>
    /// Сообщение.
    /// </summary>
    public class StreamMessage : IStreamMessage
    {
        /// <summary>
        /// Тело сообщения.
        /// </summary>
        public string Data { get; set; } = null!;

        /// <summary>
        /// Сообщение.
        /// </summary>
        /// <param name="data">Тело сообщения.</param>
        /// <exception cref="ArgumentException">Тело сообщения равно null.</exception>
        public StreamMessage(string data)
        {
            if (data == null)
            {
                throw new ArgumentException("Stream message data is null.");
            }

            Data = data;
        }
    }
}
