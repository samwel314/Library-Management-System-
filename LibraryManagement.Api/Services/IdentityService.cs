using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Identity;


namespace LibraryManagement.Api.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserActivityLogService _activityLog;

        public IdentityService(UserManager<IdentityUser> userManager, IJwtTokenService jwtTokenService, IUserActivityLogService activityLog)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _activityLog = activityLog;
        }

        public async Task<ResultT<JwtResultDto>> LoginAsync(LoginRequestDto requestDto)
        {
            var user = await _userManager.FindByNameAsync(requestDto.Email);

            if (user == null)
                return ResultT<JwtResultDto>.Failure("Invalid credentials", ErrorType.Unauthorized);

            if (await _userManager.IsLockedOutAsync(user))
                return ResultT<JwtResultDto>.Failure("Account is locked. Try later", ErrorType.Forbidden);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, requestDto.Password);

            if (!isPasswordValid)
            {
                await _activityLog.LogAsync($"Try Login (Id: {user.Id})", default, true);

                await _userManager.AccessFailedAsync(user);
                return ResultT<JwtResultDto>.Failure("Invalid credentials", ErrorType.Unauthorized);
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            await _activityLog.LogAsync($"User Login (Id: {user.Id})", default, true);
            var jwtResultDto = await _jwtTokenService.GenerateToken(user); 
            return ResultT<JwtResultDto>.Success(jwtResultDto, "Login successful");
        }

    }

}
