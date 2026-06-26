using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IBookService
    {
        Task<ResultT<int?>> CreateAsync(CreateBookDto requestDto, CancellationToken cancellation);
        Task<ResultT<IEnumerable<BookDto>>> GetAllAsync(BookFilterDto bookFilter, CancellationToken cancellation);
        Task<ResultT<BookDetailsDto>> GetByIdAsync(int id , CancellationToken cancellation);
        Task<Result> UpdateBasicInfoAsync(int id , UpdateBookBasicInfoDto requestDto, CancellationToken cancellation);
        Task<Result> UpdateAuthorsAsync(int id, UpdateBookAuthorsDto requestDto, CancellationToken cancellation);
        Task<Result> UpdateCoverImageAsync(int id, UpdateBookCoveImageDto requestDto, CancellationToken cancellation);
        Task<Result> DeleteAsync(int id, CancellationToken cancellation); 

    }
}
