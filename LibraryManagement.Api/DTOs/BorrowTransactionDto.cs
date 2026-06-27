namespace LibraryManagement.Api.DTOs
{
    public class BorrowTransactionDto
    {
        public int Id { get; set; }
        public MemberLookupDto Member { get; set; } = null!;
        public BookLookupDto Book { get; set; } = null!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
