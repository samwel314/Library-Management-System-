using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IBorrowService
    {
        Task<ResultT<int?>> BorrowBookAsync(BorrowBookRequestDto requestDto , CancellationToken cancellation);
        Task<ResultT<BorrowTransactionDto>> GetBorrowingByIdAsync(int id , CancellationToken cancellation);
        Task<Result> ReturnBorrowingByIdAsync(int id, CancellationToken cancellation);

    }
}
