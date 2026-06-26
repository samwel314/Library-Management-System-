using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class MemberRequestDto
    {
        [Required(ErrorMessage = "Member name is required.")]
        [Length(minimumLength: 1, maximumLength: 50, ErrorMessage = "Member name length between 1 : 50 chars ")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$",
            ErrorMessage = "Invalid Egyptian phone number.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100)]
        public string Email { get; set; } = null!;
    }
}
