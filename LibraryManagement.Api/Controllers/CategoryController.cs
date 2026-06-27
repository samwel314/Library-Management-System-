using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/categories")]
    [Tags("categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles = "Administrator,Librarian")]
        [EndpointName("CreateCategory")]
        [EndpointDescription("Creates a new category.")]
        [Consumes("application/json")]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status201Created)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ResultT<int?>>(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory
            (CategoryRequestDto requestDto , CancellationToken cancellation)
        {
            var result = await _categoryService.CreateAsync(requestDto, cancellation);
            if (result.IsSuccess)
                return CreatedAtRoute("GetCategoryById",new { id = result.Data },  result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.NotFound => NotFound( result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [EndpointName("GetCategoryById")]
        [EndpointDescription("Retrieves a category by its identifier.")]
        [ProducesResponseType<ResultT<CategoryDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [HttpGet ("{id}" , Name = "GetCategoryById")]
        public async Task<IActionResult> GetById(int id , CancellationToken cancellation)
        {
            var result = await _categoryService.GetByIdAsync(id, cancellation); 
            if  (result.IsSuccess)
                return Ok(result);  

            return NotFound(result);    
        }


        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [EndpointName("GetAllCategories")]
        [EndpointDescription("Retrieves all categories.")]
        [ProducesResponseType<ResultT<IEnumerable<CategoryDto>>>(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll( CancellationToken cancellation)
        {
            var result = await _categoryService.GetAllAsync( cancellation);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian,Staff")]
        [EndpointName("GetCategoryLookup")]
        [EndpointDescription("Retrieves a lightweight list of categories for lookup purposes.")]
        [ProducesResponseType<ResultT<IEnumerable<CategoryLookupDto>>>(StatusCodes.Status200OK)]
        [HttpGet ("lookup")]
        public async Task<IActionResult> GetAllLookup(CancellationToken cancellation)
        {
            var result = await _categoryService.GetAllLookupAsync(cancellation);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [EndpointName("UpdateCategory")]
        [EndpointDescription("Updates an existing category.")]
        [Consumes("application/json")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpPut ("{id}")]
        public async Task<IActionResult> UpateCategory( int id ,  CategoryRequestDto requestDto, 
            CancellationToken cancellation)
        {
            var result = await _categoryService.UpdateAsync(id ,  requestDto, cancellation);
            if (result.IsSuccess)
                return Ok( result);

            return result.ErrorType switch
            {
                ErrorType.Conflict => Conflict(result),
                ErrorType.NotFound => NotFound(result),
                ErrorType.Validation => BadRequest(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }


        [Authorize(Roles = "Administrator")]
        [EndpointName("DeleteCategory")]
        [EndpointDescription("Deletes a category if it has no books or subcategories.")]
        [ProducesResponseType<Result>(StatusCodes.Status200OK)]
        [ProducesResponseType<Result>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<Result>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<Result>(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id, CancellationToken cancellation)
        {
            var result = await _categoryService.DeleteAsync(id, cancellation);
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
