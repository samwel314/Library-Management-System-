using LibraryManagement.Api.Entities;

namespace LibraryManagement.Api.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public List<AuthorLookupDto> Authors { get; set; } = [];
        public CategoryLookupDto Category { get; set; } = null!;
        public PublisherLookupDto Publisher { get; set; } = null!;
        public BookStatus Status { get; set; }
        public string CoverImageUrl { get; set; } = null!;
    }
}
