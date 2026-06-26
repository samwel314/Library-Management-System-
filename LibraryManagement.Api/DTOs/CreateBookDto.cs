using LibraryManagement.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class CreateBookDto
    {
        [Required(ErrorMessage = "Book title is required.")]
        [Length(1, 200, ErrorMessage = "Book title length must be between 1 and 200 characters.")]
        public string Title { get; set; } = null!; 
        [Required(ErrorMessage = "Book ISBN is required.")]
        [Length(1, 30, ErrorMessage = "Book ISBN length must be between 1 and 30 characters.")]
        public  string ISBN { get; set; } = null!;
        [Required(ErrorMessage = "Book language is required.")]
        [MaxLength(50, ErrorMessage = "Book language length can not be grater than 50 characters.")]
        public  string Language { get; set; } = null!;
        [Range(1450, 2100)]
        public int PublicationYear { get; set; }
        [MaxLength( 50, ErrorMessage = "Booke edition length can not be grater than 50 characters..")]
        public string? Edition { get; set; }
        [MaxLength(1000, ErrorMessage = "Book Summary length length can not be grater than 1000 characters.")]
        public string? Summary { get; set; }
        public int PublisherId { get; set; }
        public int CategoryId { get; set; }
        [Required (ErrorMessage = "Booke should belong to at least one author ")]
        public List<int> AuthorsIds { get; set; } = null!;
        [Required(ErrorMessage = "Booke should has cover image")]
        public IFormFile CoverImage{ get; set; } = null!;

    }
}
