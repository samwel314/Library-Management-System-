namespace LibraryManagement.Api.DTOs
{
    public class BookBorrowTransactionDto
    {
        public int Id { get; set; }

        public MemberLookupDto Member { get; set; } = null!;

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
