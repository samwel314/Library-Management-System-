using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class CreateUserRequestDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = null!;
        [Required]
        [RegularExpression(@"^01[0125][0-9]{8}$")]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
    }
}
