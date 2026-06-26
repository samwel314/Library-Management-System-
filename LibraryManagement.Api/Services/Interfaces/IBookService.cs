using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IBookService
    {
        Task<ResultT<int?>> CreateAsync(CreateBookDto requestDto, CancellationToken cancellation);
        Task<ResultT<IEnumerable<BookDto>>> GetAllAsync(BookFilterDto bookFilter, CancellationToken cancellation);
        Task<ResultT<BookDetailsDto>> GetByIdAsync(int id , CancellationToken cancellation);
    }
}
