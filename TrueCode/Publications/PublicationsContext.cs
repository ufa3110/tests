namespace TrueCodeTestSolution.Publications
{
    /// <summary>
    /// Контекст (в данном случае мок) источника публикации.
    /// Интерфейс не выделен, т.к. на примере EFCore контекст наследуется от DbContext
    /// TODO: вынести методы типа GetAll в отдельный класс репозитория.
    /// </summary>
    public static class PublicationsContext
    {
        public static List<Publication> Posts { get; set;} = new List<Publication> 
        {
            new Publication("Post1", "author_1_ID", new DateTime(2023,7,5), "PostContent_1"),
            new Publication("Post2", "author_1_ID", new DateTime(2023,7,6), "PostContent_1"),
            new Publication("Post3", "author_1_ID", new DateTime(2023,7,7), "PostContent_1"),
            new Publication("Post4", "author_2_ID", new DateTime(2023,7,5), "PostContent_1"),
            new Publication("Post5", "author_2_ID", new DateTime(2023,7,6), "PostContent_1"),
            new Publication("Post6", "author_2_ID", new DateTime(2023,7,6), "PostContent_1"),
            new Publication("Post7", "author_2_ID", new DateTime(2023,7,7), "PostContent_1"),
        };

        /// <summary>
        /// Получить все публикации.
        /// </summary>
        /// <returns>Нематериализованный список публикаций.</returns>
        public static IQueryable<Publication> GetAllPosts()
        {
            return Posts.AsQueryable();
        }
    }
}
