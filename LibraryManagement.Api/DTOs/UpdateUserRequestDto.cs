using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class UpdateUserRequestDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = null!;
        [Required]
        [RegularExpression(@"^01[0125][0-9]{8}$")]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
    }
}
