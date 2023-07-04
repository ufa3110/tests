namespace TrueCodeTestSolution.TestClass
{
    /// <summary>
    /// Класс для считывания сообщений из потока.
    /// </summary>
    public class StreamMessageHelper
    {
        /// <summary>
        /// Длинна буффера для чтения из потока.
        /// </summary>
        public int BufferLength { get; private set; }

        /// <summary>
        /// Задать длинну для чтения из потока.
        /// Длинна для чтения из потока так же может быть передана как параметр (имеет приоритет) в методе ReadMessage.
        /// </summary>
        /// <param name="value">Длинна для чтения из потока.</param>
        /// <exception cref="ArgumentOutOfRangeException">Значение меньше или равно 0.</exception>
        public void SetBufferLength(int value)
        {
            //т.к. в ТЗ не указано явно, каким образом параметры должны
            //передаваться в класс - сделал оба варианта.
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException("Buffer length is negative or equals to zero.");
            }

            BufferLength = value;
        }

        /// <summary>
        /// Символ окончания сообщения.
        /// </summary>
        public char MessageEnding { get; private set; }

        /// <summary>
        /// Задать символ окончаня сообщения.
        /// Символ окончаня сообщения так же может быть передан как параметр (имеет приоритет) в методе ReadMessage.
        /// </summary>
        /// <param name="value">Символ окончаня сообщения.</param>
        public void SetMessageEnding(char value)
        {
            MessageEnding = value;
        }

        /// <summary>
        /// Оперируемый поток для извлечения сообщений.
        /// </summary>
        private Stream StreamToRead;

        /// <summary>
        /// Хелпер для извлечения сообщений из потока.
        /// </summary>
        /// <param name="stream">Поток, содержащий сообщения.</param>
        public StreamMessageHelper(Stream stream) 
        {
            this.StreamToRead = stream;
        }

        /// <summary>
        /// Считать сообщения из потока.
        /// </summary>
        /// <param name="startIndex">Указатель позиции потока.</param>
        /// <param name="ending">Символ окончания сообщения.</param>
        /// <param name="bufferLength">Длинна для считывания потока.</param>
        /// <returns>Список считанных до конца сообщений.</returns>
        /// <exception cref="InvalidDataException">Поток недоступен для чтения / закрыт.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Указанная длинна буффера меньше либо равна 0.</exception>
        /// <exception cref="DataMisalignedException">Буффер слишком мал для считывания сообщения.</exception>
        public IEnumerable<StreamMessage> ReadMessages(int startIndex = 0, char? ending = null, int? bufferLength = null)
        {
            if (!StreamToRead.CanRead)
            {
                throw new InvalidDataException("Stream is not readable.");
            }

            if (bufferLength != null && bufferLength <= 0)
            {
                throw new ArgumentOutOfRangeException("Buffer length is negative or equals to zero.");
            }

            var result = new List<byte>();
            char[] buffer = new char[bufferLength ?? this.BufferLength];

            using (StreamReader sr = new StreamReader(StreamToRead))
            {
                sr.ReadBlock(buffer, startIndex, buffer.Length);

                var strings = String.Concat(buffer)
                                    .Split(ending ?? MessageEnding)
                                    .ToList();

                if (buffer.Last() != (ending ?? MessageEnding))
                {
                    if (strings.Count <= 1)
                    {
                        throw new DataMisalignedException("Message is too long for buffer size.");
                    }

                    strings = strings.SkipLast(1).ToList();
                }

                return strings.Select(s => new StreamMessage(s));
            }
        }
    }
}
