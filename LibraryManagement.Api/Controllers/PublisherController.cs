using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using TLibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/publishers")]
    [Tags("publishers")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService  publisherService )
        {
            _publisherService = publisherService;
        }
        [EndpointName("CreatePublisher")]
        [EndpointDescription("Creates a new Publisher.")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreatePublisher
            (PublisherRequestDto requestDto, CancellationToken cancellation)
        {
            var result = await _publisherService.CreateAsync(requestDto, cancellation);
            if (result.IsSuccess)
                return CreatedAtRoute("GetPublisherById", new { id = result.Data }, result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        [EndpointName("GetPublisherById")]
        [EndpointDescription("Retrieves a Publisher by its identifier.")]
        [ProducesResponseType<ResultT<PublisherDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet("{id}", Name = "GetPublisherById")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellation)
        {
            var result = await _publisherService.GetByIdAsync(id, cancellation);
            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }
        [EndpointName("GetAllPublishers")]
        [EndpointDescription("Retrieves all publishers.")]
        [ProducesResponseType<ResultT<IEnumerable<PublisherDto>>>(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _publisherService.GetAllAsync(cancellation);
            return Ok(result);
        }
        [EndpointName("GetpublishersLookup")]
        [EndpointDescription("Retrieves a lightweight list of publishers for lookup purposes.")]
        [ProducesResponseType<ResultT<IEnumerable<PublisherLookupDto>>>(StatusCodes.Status200OK)]
        [HttpGet("lookup")]
        public async Task<IActionResult> GetAllLookup(CancellationToken cancellation)
        {
            var result = await _publisherService.GetAllLookupAsync(cancellation);
            return Ok(result);
        }
        [EndpointName("UpdatePublisher")]
        [EndpointDescription("Updates an existing Publisher.")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, PublisherRequestDto requestDto,
            CancellationToken cancellation)
        {
            var result = await _publisherService.UpdateAsync(id, requestDto, cancellation);
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

        [EndpointName("DeletePublisher")]
        [EndpointDescription("Deletes a Publisher if it has no books.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken cancellation)
        {
            var result = await _publisherService.DeleteAsync(id, cancellation);
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
