using LibraryManagement.Api.Entities;

namespace LibraryManagement.Api.DTOs
{
    public class BookDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public string Language { get; set; } = null!;
        public int PublicationYear { get; set; }
        public string? Edition { get; set; }
        public string? Summary { get; set; }
        public List<AuthorLookupDto> Authors { get; set; } = [];
        public CategoryDto Category { get; set; } = null!;
        public PublisherDto Publisher { get; set; } = null!;
        public BookStatus Status { get; set; }
        public string CoverImageUrl { get; set; } = null!;
    }
}
