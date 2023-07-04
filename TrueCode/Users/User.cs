namespace TrueCodeTestSolution.Users
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; private set; }

        public User(string id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }
}
