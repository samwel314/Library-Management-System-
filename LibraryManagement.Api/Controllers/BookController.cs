using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/books")]
    [Tags("books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [EndpointName("CreateBook")]
        [EndpointDescription("Creates a new book.")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateBook
        ([FromForm] CreateBookDto requestDto, CancellationToken cancellation)
        {
            var result = await _bookService.CreateAsync(requestDto, cancellation);
            if (result.IsSuccess)
                return CreatedAtRoute("GetBookById", new { id = result.Data }, result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        [EndpointName("GetBookById")]
        [EndpointDescription("Retrieves a book by its identifier.")]
        [ProducesResponseType<ResultT<BookDetailsDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetBookById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result  = await _bookService.GetByIdAsync(id, cancellation);  
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [EndpointName("GetAllBooks")]
        [EndpointDescription("Retrieves all books.")]
        [ProducesResponseType<ResultT<IEnumerable<BookDto>>>(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll( [FromQuery] BookFilterDto bookFilter , CancellationToken cancellation)
        {
            var result = await _bookService.GetAllAsync(bookFilter ,cancellation);
            return Ok(result);
        }

        [EndpointName("UpdateBookBasicInfo")]
        [EndpointDescription("Updates an existing book basic info.")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasicInfo(int id, UpdateBookBasicInfoDto requestDto,
               CancellationToken cancellation)
        {
            var result = await _bookService.UpdateBasicInfoAsync(id, requestDto, cancellation);
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

        [EndpointName("UpdateBookAuthors")]
        [EndpointDescription("Updates an existing book authors .")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/authors")]
        public async Task<IActionResult> UpdateBookAuthors(int id, UpdateBookAuthorsDto requestDto,
            CancellationToken cancellation)
        {
            var result = await _bookService.UpdateAuthorsAsync(id, requestDto, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [EndpointName("UpdateBookCoverImage")]
        [EndpointDescription("Updates an existing book cover image  .")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}/cover-image")]
        public async Task<IActionResult> UpdateBookCoverImage(int id, [FromForm] UpdateBookCoveImageDto requestDto,
            CancellationToken cancellation)
        {
            var result = await _bookService.UpdateCoverImageAsync(id, requestDto, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        [EndpointName("DeleteBook")]
        [EndpointDescription("Deletes a Book if it has no borrow history.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken cancellation)
        {
            var result = await _bookService.DeleteAsync(id, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        [EndpointName("GetBookBorrowHistory")]
        [EndpointDescription("Retrieves the borrowing history of a book.")]
        [ProducesResponseType<ResultT<IEnumerable<BookBorrowTransactionDto>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/borrowings")]
        public async Task<IActionResult> GetBorrowHistory(int id, CancellationToken cancellation)
        {
            var result = await _bookService.GetBorrowHistoryAsync(id, cancellation);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }
    }
}
