using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/authors")]
    [Tags("authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [Authorize(Roles = "Administrator,Librarian")]
        [EndpointName("CreateAuthor")]
        [EndpointDescription("Creates a new Author.")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor
            (AuthorRequestDto requestDto, CancellationToken cancellation)
        {
            var result = await _authorService.CreateAsync(requestDto, cancellation);
            if (result.IsSuccess)
                return CreatedAtRoute("GetAuthorById", new { id = result.Data }, result);

            return result.ErrorType switch
            {
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [EndpointName("GetAuthorById")]
        [EndpointDescription("Retrieves a Author by its identifier.")]
        [ProducesResponseType<ResultT<AuthorDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetAuthorById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _authorService.GetByIdAsync(id, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [EndpointName("GetAllAuthors")]
        [EndpointDescription("Retrieves all authors.")]
        [ProducesResponseType<ResultT<IEnumerable<AuthorDto>>>(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _authorService.GetAllAsync(cancellation);
            return Ok(result);
        }
        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [EndpointName("GetAuthorsLookup")]
        [EndpointDescription("Retrieves a lightweight list of authors for lookup purposes.")]
        [ProducesResponseType<ResultT<IEnumerable<AuthorLookupDto>>>(StatusCodes.Status200OK)]
        [HttpGet("lookup")]
        public async Task<IActionResult> GetAllLookup(CancellationToken cancellation)
        {
            var result = await _authorService.GetAllLookupAsync(cancellation);
            return Ok(result);
        }
        [Authorize(Roles = "Administrator,Librarian")]
        [EndpointName("UpdateAuthor")]
        [EndpointDescription("Updates an existing Author.")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, AuthorRequestDto requestDto,
            CancellationToken cancellation)
        {
            var result = await _authorService.UpdateAsync(id, requestDto, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
        [Authorize(Roles = "Administrator")]
        [EndpointName("DeleteAuthor")]
        [EndpointDescription("Deletes an author if the author has no associated books.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken cancellation)
        {
            var result = await _authorService.DeleteAsync(id, cancellation);
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
