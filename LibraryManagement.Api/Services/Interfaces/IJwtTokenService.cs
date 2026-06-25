using LibraryManagement.Api.DTOs;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IJwtTokenService
    {
        Task<JwtResultDto> GenerateToken(IdentityUser identity);
    }
}
