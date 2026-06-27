using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IMemberService
    {
        Task<ResultT<int?>> CreateAsync(MemberRequestDto requestDto, CancellationToken cancellation);
        Task<ResultT<IEnumerable<MemberDto>>> GetAllAsync(CancellationToken cancellation);
        Task<ResultT<MemberDto>> GetByIdAsync(int id, CancellationToken cancellation);
        Task<Result> UpdateAsync(int id, MemberRequestDto requestDto, CancellationToken cancellation);
        Task<Result> DeleteAsync(int id, CancellationToken cancellation);
        Task<ResultT<IEnumerable<MemberBorrowTransactionDto>>> GetBorrowHistoryAsync(int id, CancellationToken cancellation);
    }
}
