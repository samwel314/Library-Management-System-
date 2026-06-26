using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ResultT<int?>> CreateAsync(CategoryRequestDto requestDto , CancellationToken cancellation);
        Task<ResultT<IEnumerable<CategoryLookupDto>>> GetAllLookupAsync(CancellationToken cancellation);
        Task<ResultT<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellation);
        Task<ResultT<CategoryDto>> GetByIdAsync(int id, CancellationToken cancellation);
        Task<Result> UpdateAsync(int id, CategoryRequestDto requestDto, CancellationToken cancellation);
        Task<Result> DeleteAsync(int id, CancellationToken cancellation);
    }

}
