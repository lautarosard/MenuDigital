using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IDish;
using Application.Models.Request;
using Application.Models.Response;
using MenuDigital.Exeptions;
using MenuDigital.Exceptions;

namespace MenuDigital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }
        //add exeptions handling =====>
        // POST
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDish([FromBody] DishRequest dishRequest)
        { 
            if (dishRequest == null)
            {
                throw new RequiredParameterException("Required dish data.");
            }
            if (string.IsNullOrWhiteSpace(dishRequest.Name))
            {
                throw new RequiredParameterException("Name is required.");
            }
            if (dishRequest.CategoryId == 0)
            {
                throw new RequiredParameterException("Category is required.");
            }
            if (dishRequest.Price <= 0)
            {
                throw new InvalidParameterException("Price must be greater than zero.");
            }

            var createdDish = await _dishService.CreateDish(dishRequest);
            // Si ya existe un plato con ese nombre, devolver error 409
            if (createdDish == null)
            {
                throw new ConflictException("A dish with this name already exists.");
            }
            return CreatedAtAction(nameof(GetDishById), new { id = createdDish.Id }, createdDish);

        }
        // GETs
        // GET with filters
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<DishResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] int? categoryId, [FromQuery] string? orderPrice, [FromQuery] bool onlyActive = true)
        {

            if (!string.IsNullOrWhiteSpace(orderPrice))
            {
                var normalized = orderPrice.Trim().ToUpperInvariant();
                if (normalized != "ASC" && normalized != "DESC")
                {
                    throw new OrderPriceException("Invalid order. Use ASC or DESC.");
                }
            }
            var list = await _dishService.SearchAsync(name, categoryId, orderPrice);
            if (list == null || !list.Any())
            {
                throw new NotFoundException("No dishes found matching the criteria.");
            }
            if(onlyActive)
            {
                list = list.Where(d => d.Available);
            }
            return Ok(list);
            
        }
        //general GET
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllDishes()
        {
            var dishes = await _dishService.GetAllDishesAsync();
            if (dishes == null || !dishes.Any())
            {
                return NotFound("No dishes found.");
            }
            return new JsonResult(dishes);
        }
        //
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDishById(Guid id)
        {
            var dish = await _dishService.GetDishById(id);
            if (dish == null)
            {
                throw new NotFoundException($"Dish with ID {id} not found.");
            }
            return new JsonResult(dish);
        }


        // PUT
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDish(Guid id, [FromBody] DishRequest dishRequest)
        {
            if (dishRequest == null)
            {
                throw new RequiredParameterException("Required dish data.");
            }
            if (string.IsNullOrWhiteSpace(dishRequest.Name))
            {
                throw new RequiredParameterException("Name is required.");
            }
            if (dishRequest.CategoryId == 0)
            {
                throw new RequiredParameterException("Category is required.");
            }
            if (string.IsNullOrWhiteSpace(dishRequest.ImageUrl))
            {
                throw new RequiredParameterException("ImageUrl is required.");
            }
            if (dishRequest.Price <= 0)
            {
                throw new InvalidParameterException("Price must be greater than zero.");
            }
            var updatedDish = await _dishService.UpdateDish(id, dishRequest);
            if (updatedDish == null)
            {
                throw new NotFoundException($"Dish with ID {id} not found.");
            }
            return new JsonResult(updatedDish);
        }

        // DELETE
    }
}
