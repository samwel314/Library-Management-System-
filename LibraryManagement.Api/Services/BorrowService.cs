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
        private readonly IUserActivityLogService _activityLog;

        public BorrowService(LibraryDbContext db, IUserActivityLogService activityLog)
        {
            _db = db;
            _activityLog = activityLog;
        }

        public async Task<ResultT<int?>> BorrowBookAsync(BorrowBookRequestDto requestDto, CancellationToken cancellation)
        {
            if (requestDto.DueDate.Date <= DateTime.Today)
                return ResultT<int?>.Failure("Due date must be after today.", ErrorType.Validation);

            var isValidMember = await _db.Members.AnyAsync(m => m.Id == requestDto.MemberId, cancellation);
            if (!isValidMember)
                return ResultT<int?>.Failure("Member not found", ErrorType.NotFound);

            var book = await _db.Books.FindAsync(requestDto.BookId, cancellation);
            if (book is null)
                return ResultT<int?>.Failure("Book not found", ErrorType.NotFound);
            if (book.Status == BookStatus.Out)
                return ResultT<int?>.Failure("This book not available at this time", ErrorType.Conflict);

            var borrowTransaction = new BorrowTransaction
            {
                BookId = requestDto.BookId,
                MemberId = requestDto.MemberId,
                DueDate = requestDto.DueDate,
            };
            await _db.BorrowTransactions.AddAsync(borrowTransaction, cancellation);
            book.Status = BookStatus.Out;
            await _activityLog.LogAsync($"Book Borrowed", cancellation);
            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(borrowTransaction.Id, "Book borrowing successfully");
        }

        public async Task<ResultT<IEnumerable<BorrowTransactionDto>>> GetActiveBorrowingsAsync(CancellationToken cancellation)
        {
            var activeBorrowTransactions = await _db.BorrowTransactions.AsNoTracking()
                .Where(br => br.ReturnDate == null).Select(br => new BorrowTransactionDto
            {
                Id = br.Id,
                BorrowDate = br.BorrowDate,
                DueDate = br.DueDate,
                ReturnDate = br.ReturnDate,
                Member = new MemberLookupDto
                {
                    Id = br.MemberId,
                    Name = br.Member.Name,
                },
                Book = new BookLookupDto
                {
                    Id = br.BookId,
                    Title = br.Book.Title,
                }
            }).ToListAsync(cancellation);

            return ResultT<IEnumerable<BorrowTransactionDto>>.Success(activeBorrowTransactions); 
        }

        public async Task<ResultT<BorrowTransactionDto>> GetBorrowingByIdAsync(int id, CancellationToken cancellation)
        {
            var borrowTransaction =
                await _db.BorrowTransactions.Where(br => br.Id == id).OrderBy(br => br.DueDate)
                .Select(br => new BorrowTransactionDto
                {
                    Id = br.Id,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    ReturnDate = br.ReturnDate,
                    Member = new MemberLookupDto
                    {
                        Id = br.MemberId,
                        Name = br.Member.Name,
                    } , 
                    Book = new BookLookupDto
                    {
                        Id = br.BookId, 
                        Title = br.Book.Title,  
                    }
                }).FirstOrDefaultAsync();

            if (borrowTransaction is null)
                return ResultT<BorrowTransactionDto>.Failure("Transaction not found", ErrorType.NotFound);

            return ResultT<BorrowTransactionDto>.Success(borrowTransaction);
        }

        public async Task<Result> ReturnBorrowingByIdAsync(int id, CancellationToken cancellation)
        {
           var  borrowTransaction =await _db.BorrowTransactions
                .Include(br => br.Book).FirstOrDefaultAsync(br => br.Id == id ,cancellation);
            if (borrowTransaction is null)
                return Result.Failure("Transaction not found", ErrorType.NotFound);
            if (borrowTransaction.ReturnDate != null )
                return Result.Failure("Borrow already returned", ErrorType.Conflict);
            borrowTransaction.ReturnDate = DateTime.UtcNow;
            borrowTransaction.Book.Status = BookStatus.In;
            await _activityLog.LogAsync( $"Book Returned (Transaction Id: {borrowTransaction.Id})", cancellation);
            await  _db.SaveChangesAsync(cancellation);
            return Result.Success("Borrow returned successfully");
        }
    }
}
