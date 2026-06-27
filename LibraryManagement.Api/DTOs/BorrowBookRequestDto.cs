using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.DTOs
{
    public class BorrowBookRequestDto
    {
        public int BookId { get; set; }        
        public int MemberId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
