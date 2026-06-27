using LibraryManagement.Api.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryManagement.Api.DTOs
{
    public class BookFilterDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        [ValidateNever]
        public BookStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
