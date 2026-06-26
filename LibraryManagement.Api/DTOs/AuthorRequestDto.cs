using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class AuthorRequestDto
    {
        [Required(ErrorMessage = "Author name is required.")]
        [Length(1, 100, ErrorMessage = "Author name length must be between 1 and 100 characters.")]
        public string Name { get; set; } = null!;
        [MaxLength( 1000, ErrorMessage = "Bio cannot exceed 1000 characters.")]
        public string? Bio { get; set; }
    }

}
