namespace LibraryManagement.Api.DTOs
{
    public class MemberBorrowTransactionDto
    {
        public int Id { get; set; }

        public BookLookupDto Book { get; set; } = null!;

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
