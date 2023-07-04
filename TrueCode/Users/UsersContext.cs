namespace TrueCodeTestSolution.Users
{
    /// <summary>
    /// Контекст пользователей.
    /// </summary>
    public static class UsersContext
    {
        public static List<User> Users { get; set; } = new List<User>() 
        {
            new User("author_1_ID", "User1"),
            new User("author_2_ID", "User2"),
        };

        /// <summary>
        /// Получить пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Пользователь с идентификатором.</returns>
        /// <exception cref="InvalidOperationException">Пользователь не найден.</exception>
        /// <exception cref="ArgumentNullException">Контекст пользователей не существует.</exception>
        public static User GetUserByID(string id)
        {
            return Users!.Single(user => user!.ID == id);
        }

    }
}
