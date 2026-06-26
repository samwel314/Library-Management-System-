using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class CategoryRequestDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [Length (minimumLength: 1 , maximumLength: 50 ,ErrorMessage = "category name length between 1 : 50 chars ") ]
        public string Name { get; set; } = null!; 
        public int ? ParentId { get; set; } 
    }
}
