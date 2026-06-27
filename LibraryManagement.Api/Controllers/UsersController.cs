using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/users")]
    [Tags("users")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [EndpointName("CreateUser")]
        [EndpointDescription("Creates a new system user (Staff, Librarian, Administrator).")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<string>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ResultT<string>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<string>>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ResultT<string>>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto requestDto , CancellationToken cancellation)
        {
            var result = await _userService.CreateAsync(requestDto ,cancellation );
            if (result.IsSuccess)
                return CreatedAtRoute("GetUserById", new { id = result.Data }, result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpGet]
        [EndpointName("GetAllUsers")]
        [EndpointDescription("Retrieves all system users.")]
        [ProducesResponseType<ResultT<IEnumerable<UserDto>>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _userService.GetAllAsync(cancellation);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [EndpointName("GetUserById")]
        [EndpointDescription("Retrieves a system user by their identifier.")]
        [ProducesResponseType<ResultT<UserDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPut("{id}")]
        [EndpointName("UpdateUser")]
        [EndpointDescription("Updates an existing system user's information and role.")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserRequestDto requestDto , CancellationToken cancellation)
        {
            var result = await _userService.UpdateAsync(id, requestDto ,cancellation );
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [HttpDelete("{id}")]
        [EndpointName("DeleteUser")]
        [EndpointDescription("Deletes a system user from the system.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteById([FromRoute] string id , CancellationToken cancellation)
        {
            var result = await _userService.DeleteAsync(id , cancellation) ;
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}
