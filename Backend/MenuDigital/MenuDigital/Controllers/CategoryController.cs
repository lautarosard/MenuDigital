using Application.Interfaces.ICategory;
using Application.Models.Response;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenuDigital.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private readonly IGetAllCategoryAsyncUseCase _getAllCategoryAsyncUseCase;

        public CategoryController(IGetAllCategoryAsyncUseCase getAllCategoryAsyncUseCase)
        {
            _getAllCategoryAsyncUseCase = getAllCategoryAsyncUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DeliveryTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _getAllCategoryAsyncUseCase.GetAllAsync();
            return Ok(categories);
        }
    }
}
