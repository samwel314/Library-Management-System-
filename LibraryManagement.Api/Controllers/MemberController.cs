using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/members")]
    [Tags("members")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [EndpointName("CreateMember")]
        [EndpointDescription("Creates a new Member.")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateMember(
            MemberRequestDto requestDto,
            CancellationToken cancellation)
        {
            var result = await _memberService.CreateAsync(requestDto, cancellation);

            if (result.IsSuccess)
                return CreatedAtRoute("GetMemberById", new { id = result.Data }, result);

            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [EndpointName("GetMemberById")]
        [EndpointDescription("Retrieves a Member by its identifier.")]
        [ProducesResponseType<ResultT<MemberDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetMemberById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _memberService.GetByIdAsync(id, cancellation);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [EndpointName("GetAllMembers")]
        [EndpointDescription("Retrieves all members.")]
        [ProducesResponseType<ResultT<IEnumerable<MemberDto>>>(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _memberService.GetAllAsync(cancellation);
            return Ok(result);
        }

        [EndpointName("UpdateMember")]
        [EndpointDescription("Updates an existing Member.")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(
            int id,
            MemberRequestDto requestDto,
            CancellationToken cancellation)
        {
            var result = await _memberService.UpdateAsync(id, requestDto, cancellation);

            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [EndpointName("DeleteMember")]
        [EndpointDescription("Deletes a Member if it has no borrowing history.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken cancellation)
        {
            var result = await _memberService.DeleteAsync(id, cancellation);

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
