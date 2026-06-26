using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<ResultT<int?>> CreateAsync(PublisherRequestDto requestDto, CancellationToken cancellation);
        Task<ResultT<IEnumerable<PublisherLookupDto>>> GetAllLookupAsync(CancellationToken cancellation);
        Task<ResultT<IEnumerable<PublisherDto>>> GetAllAsync(CancellationToken cancellation);
        Task<ResultT<PublisherDto>> GetByIdAsync(int id, CancellationToken cancellation);
        Task<Result> UpdateAsync(int id, PublisherRequestDto requestDto, CancellationToken cancellation);
        Task<Result> DeleteAsync(int id, CancellationToken cancellation);
    }
}
