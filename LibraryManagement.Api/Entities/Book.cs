namespace LibraryManagement.Api.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string ISBN { get; set; }
        public required string Language { get; set; }
        public int PublicationYear { get; set; }
        public BookStatus Status { get; set; }
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public string? CoverImageUrl { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = new List<BorrowTransaction>();
    }
}
