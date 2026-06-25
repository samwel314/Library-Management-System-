using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<Result> LoginAsync(LoginRequestDto requestDto); 
    }
}
