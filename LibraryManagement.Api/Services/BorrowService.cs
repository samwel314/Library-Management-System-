using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly LibraryDbContext _db;

        public BorrowService(LibraryDbContext db)
        {
            _db = db;
        }

        public  async Task<ResultT<int?>> BorrowBookAsync(BorrowBookRequestDto requestDto, CancellationToken cancellation)
        {
            if (requestDto.DueDate.Date <= DateTime.Today)
                return ResultT< int?>.Failure("Due date must be after today.",ErrorType.Validation);

            var isValidMember =await _db.Members.AnyAsync(m => m.Id == requestDto.MemberId , cancellation); 
            if (!isValidMember)
                return ResultT<int?>.Failure("Member not found", ErrorType.NotFound);

            var book = await _db.Books.FindAsync(requestDto.BookId ,cancellation);
            if (book is null )
                return ResultT<int?>.Failure("Book not found", ErrorType.NotFound);
            if (book.Status == BookStatus.Out)
                return ResultT<int?>.Failure("This book not available at this time", ErrorType.Conflict);

            var borrowTransaction = new BorrowTransaction
            {
                BookId = requestDto.BookId,
                MemberId = requestDto.MemberId,
                DueDate = requestDto.DueDate,
            }; 
            await _db.BorrowTransactions.AddAsync (borrowTransaction ,cancellation);
            book.Status = BookStatus.Out;   
            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(borrowTransaction.Id, "Book borrowing successfully"); 
        }
    }
}
