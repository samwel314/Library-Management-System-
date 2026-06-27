using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/borrowings")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowService _borrowServices;

        public BorrowController(IBorrowService borrowServices)
        {
            _borrowServices = borrowServices;
        }

        [EndpointName("BorrowBook")]
        [EndpointDescription("Borrows a book for a library member.")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> BorrowBook(BorrowBookRequestDto requestDto, CancellationToken cancellation)
        {
            var result = await _borrowServices.BorrowBookAsync(requestDto, cancellation);
            if (result.IsSuccess)
                return CreatedAtRoute("GetBorrowingById", new { id = result.Data }, result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        [EndpointDescription("Retrieves a borrowing transaction by its identifier..")]
        [ProducesResponseType<ResultT<BorrowTransactionDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetBorrowingById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _borrowServices.GetBorrowingByIdAsync(id, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }
        [EndpointName("ReturnBorrowing")]
        [EndpointDescription("Returns a borrowed book by its borrowing transaction identifier.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBorrowing(int id, CancellationToken cancellation)
        {
            var result = await _borrowServices.ReturnBorrowingByIdAsync(id, cancellation); 
            if (result.IsSuccess)
                return Ok( result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.NotFound => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}
