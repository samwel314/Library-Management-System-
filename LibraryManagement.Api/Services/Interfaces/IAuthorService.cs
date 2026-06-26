using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<ResultT<int?>> CreateAsync(AuthorRequestDto requestDto, CancellationToken cancellation);
        Task<ResultT<IEnumerable<AuthorLookupDto>>> GetAllLookupAsync(CancellationToken cancellation);
        Task<ResultT<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken cancellation);
        Task<ResultT<AuthorDto>> GetByIdAsync(int id, CancellationToken cancellation);
        Task<Result> UpdateAsync(int id, AuthorRequestDto requestDto, CancellationToken cancellation);
        Task<Result> DeleteAsync(int id, CancellationToken cancellation);
    }
}
