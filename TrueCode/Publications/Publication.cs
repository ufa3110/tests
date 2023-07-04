namespace TrueCodeTestSolution.Publications
{
    /// <summary>
    /// Публикация пользователя.
    /// </summary>
    public class Publication
    {
        /// <summary>
        /// Идентификатор публикации.
        /// </summary>
        public string ID { get; private set; } = null!;

        /// <summary>
        /// Идентификатор автора публикации.
        /// </summary>
        public string AuthorID { get; private set; } = null!;

        /// <summary>
        /// Дата и время публикации.
        /// </summary>
        public DateTime PublicationDate { get; private set; } = default!;

        /// <summary>
        /// Содержание публикации.
        /// </summary>
        public string Content { get; private set; } = null!;

        public Publication(string id, string authorID, DateTime publicationDate, string content)
        {
            this.ID = id;
            this.AuthorID = authorID;
            this.PublicationDate = publicationDate;
            this.Content = content;
        }
    }
}
