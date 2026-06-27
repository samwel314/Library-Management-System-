using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tricycle.Api.Controllers
{
    [Route("api/auth")]
    [Tags("Identity")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService; 

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [EndpointName("UserLogin")]
        [EndpointDescription("Authenticates a user using email and password and returns a JWT access token.")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<JwtResultDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<Result>(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto  requestDto )
        {
            var result = await _identityService.LoginAsync(requestDto);
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.Unauthorized => Unauthorized(result),
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };

        }
    }

}

