namespace LibraryManagement.Api.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Bio { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
