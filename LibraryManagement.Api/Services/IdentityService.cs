using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Identity;


namespace TLibraryManagement.Api.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public IdentityService(UserManager<IdentityUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result> LoginAsync(LoginRequestDto requestDto)
        {
            var user = await _userManager.FindByNameAsync(requestDto.Email);

            if (user == null)
                return Result.Failure("Invalid credentials", ErrorType.Unauthorized);

            if (await _userManager.IsLockedOutAsync(user))
                return Result.Failure("Account is locked. Try later", ErrorType.Forbidden);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, requestDto.Password);

            if (!isPasswordValid)
            {
                await _userManager.AccessFailedAsync(user);
                return Result.Failure("Invalid credentials", ErrorType.Unauthorized);
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var jwtResultDto = await _jwtTokenService.GenerateToken(user); 
            return ResultT<JwtResultDto>.Success(jwtResultDto, "Login successful");
        }

    }
}
