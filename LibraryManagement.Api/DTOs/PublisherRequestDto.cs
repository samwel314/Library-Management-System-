using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class PublisherRequestDto
    {
        [Required(ErrorMessage = "Publisher name is required.")]
        [Length(minimumLength: 1, maximumLength: 50, ErrorMessage = "Publisher name length between 1 : 50 chars ")]
        public string Name { get; set; } = null!;
        [MaxLength(50 ,  ErrorMessage = "Country length can not be grater than 50 char")]
        public string? Country { get; set; }
        [MaxLength(200, ErrorMessage = "ContactInfo length can not be grater than 200 char")]
        public string? ContactInfo { get; set; }
    }
}
