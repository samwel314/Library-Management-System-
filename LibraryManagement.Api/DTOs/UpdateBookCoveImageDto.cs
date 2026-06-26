using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class UpdateBookCoveImageDto
    {
        [Required(ErrorMessage = "Booke should has cover image")]
        public IFormFile CoverImage { get; set; } = null!;

    }
}
