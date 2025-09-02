using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IDish;
using Application.Models.Request;
using Application.Models.Response;
using MenuDigital.Exeptions;
using MenuDigital.Exceptions;
using Application.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace MenuDigital.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        // POST
        /// <summary>
        /// Crea un nuevo plato en el menú del restaurante.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite registrar un nuevo plato con sus detalles.
        /// </remarks>
        [HttpPost]
        [SwaggerOperation(
        Summary = "Crear nuevo plato",
        Description = "Crea un nuevo plato en el menú del restaurante."
        )]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
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
            if (dishRequest.Category == 0)
            {
                throw new RequiredParameterException("Category is required.");
            }
            if (dishRequest.Price <= 0)
            {
                throw new InvalidParameterException("Price must be greater than zero.");
            }

            var createdDish = await _dishService.CreateDish(dishRequest);
            // if already exist a dish with that name, throw a 409 Conflict 
            if (createdDish == null)
            {
                throw new ConflictException("A dish with this name already exists.");
            }
            return CreatedAtAction(nameof(GetDishById), new { id = createdDish.Id }, createdDish);

        }
        // GETs
        // GET with filters
        /// <summary>
        /// Busca platos con opciones de filtrado y ordenamiento.
        /// </summary>
        /// <remarks>
        /// Obtiene una lista de platos del menú que coinciden con los criterios de búsqueda.
        /// </remarks>
        //("search")
        [HttpGet]
        [SwaggerOperation(
        Summary = "Buscar platos",
        Description = "Obtiene una lista de platos del menú con opciones de filtrado y ordenamiento."
        )]
        [ProducesResponseType(typeof(IEnumerable<DishResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search(
            [FromQuery] string? name, 
            [FromQuery] int? categoryId, 
            [FromQuery] OrderPrice? orderPrice = OrderPrice.ASC, 
            [FromQuery] bool onlyActive = true)
        {

            /*if (!string.IsNullOrWhiteSpace(orderPrice))
            {
                var normalized = orderPrice.Trim().ToUpperInvariant();
                if (normalized != "ASC" && normalized != "DESC")
                {
                    throw new OrderPriceException("Invalid order. Use ASC or DESC.");
                }
            }*/
            var list = await _dishService.SearchAsync(name, categoryId, orderPrice);
            if (list == null || !list.Any())
            {
                throw new NotFoundException("No dishes found matching the criteria.");
            }
            if(onlyActive)
            {
                list = list.Where(d => d.isActive);
            }
            return Ok(list);
            
        }
        /*
        //general GET
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllDishes()
        {
            var dishes = await _dishService.GetAllDishesAsync();
            if (dishes == null || !dishes.Any())
            {
                throw new NotFoundException("No dishes found.");
            }
            return new JsonResult(dishes);
        }*/
        //
        /// <summary>
        /// Obtiene un plato por su ID.
        /// </summary>
        /// <remarks>
        /// Busca un plato específico en el menú usando su identificador único.
        /// </remarks>
        [HttpGet("{id}")]
        [SwaggerOperation(
        Summary = "Buscar platos por ID",
        Description = "Buscar platos por ID."
        )]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Actualiza un plato existente en el menú.
        /// </summary>
        /// <remarks>
        /// Actualiza todos los campos de un plato existente.
        /// </remarks>

        [HttpPut("{id}")]
        [SwaggerOperation(
        Summary = "Actualizar plato existente",
        Description = "Actualiza todos los campos de un plato existente en el menú."
        )]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateDish(Guid id, [FromBody] DishUpdateRequest dishRequest)
        {
            if (dishRequest == null)
            {
                throw new RequiredParameterException("Required dish data.");
            }
            if (string.IsNullOrWhiteSpace(dishRequest.Name))
            {
                throw new RequiredParameterException("Name is required.");
            }
            if (dishRequest.Category == 0)
            {
                throw new RequiredParameterException("Category is required.");
            }
            if (dishRequest.Price <= 0)
            {
                throw new InvalidParameterException("Price must be greater than zero.");
            }

            var result = await _dishService.UpdateDish(id, dishRequest);
            if (result.NotFound)
            {
                throw new NotFoundException($"Dish with ID {id} not found.");
            }

            if(result.NameConflict)
            {
                throw new ConflictException($"dish {dishRequest.Name} already exists");
            }

            return Ok(result.UpdatedDish);
        }

        // DELETE
    }
}
