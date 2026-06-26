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
        [EndpointName("BookCategory")]
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
    }
}
