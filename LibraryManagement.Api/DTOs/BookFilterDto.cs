using LibraryManagement.Api.Entities;

namespace LibraryManagement.Api.DTOs
{
    public class BookFilterDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public BookStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
