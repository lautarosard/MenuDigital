using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Asp.Versioning;
using Application.Models.Response.Dish;

namespace MenuDigital.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class DishController : ControllerBase
    {
        private readonly ICreateDishUseCase _createDish;
        private readonly IUpdateDishUseCase _UpdateDish;
        private readonly ISearchAsyncUseCase _SearchAsync;
        private readonly ICategoryExistUseCase _CategoryExist;
        private readonly IGetDishByIdUseCase _getDishByIdUseCase;
        public DishController(
            ICreateDishUseCase createDish,
            IUpdateDishUseCase UpdateDish,
            ISearchAsyncUseCase SearchAsync,
            ICategoryExistUseCase CategoryExist)
        {
            _createDish = createDish;
            _UpdateDish = UpdateDish;
            _SearchAsync = SearchAsync;
            _CategoryExist = CategoryExist;
        }

        // POST
        /// <summary>
        /// Crear nuevo plato.
        /// </summary>
        /// <remarks>
        /// Crea un nuevo plato en el menú del restaurante.
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
            
            var categoryExists = await _CategoryExist.CategoryExist(dishRequest.Category);
            //mover al service
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {dishRequest.Category} not found.");
            }
            var createdDish = await _createDish.CreateDish(dishRequest);
            
            return CreatedAtAction(nameof(GetDishById), new { id = createdDish.Id }, createdDish);

        }
        // GETs
        // GET with filters
        /// <summary>
        /// Busca platos.
        /// </summary>
        /// <remarks>
        /// Obtiene una lista de platos del menú con opciones de filtrado y ordenamiento.
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
            [FromQuery] int? category,
            [FromQuery] OrderPrice? sortByPrice = OrderPrice.ASC,
            [FromQuery] bool? onlyActive = null)
        {
            if (category != 0 && category != null)
            {
                var categoryExists = await _CategoryExist.CategoryExist(category.Value);
                if (!categoryExists)
                {
                    throw new NotFoundException($"Category with ID {category} not found.");
                }
            }
            var list = await _SearchAsync.SearchAsync(name, category, sortByPrice, onlyActive);
            //if (list == null || !list.Any())
            //{
            //    throw new NotFoundException("No dishes found matching the criteria.");
            //}combiene que retorne una lista vacia

            return Ok(list);

        }

        //GET by ID
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
        private async Task<IActionResult> GetDishById(Guid id)
        {
            var dish = await _getDishByIdUseCase.GetDishById(id);
            return Ok(dish);
        }


        // PUT
        /// <summary>
        /// Actualizar plato existente.
        /// </summary>
        /// <remarks>
        /// Actualiza todos los campos de un plato existente en el menú.
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
            var categoryExists = await _CategoryExist.CategoryExist(dishRequest.Category);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {dishRequest.Category} not found.");
            }
            var result = await _UpdateDish.UpdateDish(id, dishRequest);

            return Ok(result);
        }

        // DELETE
        /// <summary>
        /// Eliminar plato
        /// </summary>
        /// <remarks>
        /// Elimina un plato del menú del restaurante.
        /// </remarks>
        /// 
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    }
}

