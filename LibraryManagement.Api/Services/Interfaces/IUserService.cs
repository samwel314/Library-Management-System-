using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResultT<string>> CreateAsync(CreateUserRequestDto requestDto , CancellationToken cancellation);
        Task<ResultT<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellation);
        Task<ResultT<UserDto>> GetByIdAsync(string id);
        Task<Result> UpdateAsync(string id, UpdateUserRequestDto requestDto  , CancellationToken cancellation);
        Task<Result> DeleteAsync(string id, CancellationToken cancellation);
    }
}
