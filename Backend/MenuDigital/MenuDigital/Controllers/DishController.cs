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

namespace MenuDigital.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly ICreateDishUseCase _createDish;
        private readonly IUpdateDishUseCase _UpdateDish;
        private readonly ISearchAsyncUseCase _SearchAsync;
        private readonly ICategoryExistUseCase _CategoryExist;
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
            ////if (dishRequest == null)
            ////{
            ////    throw new RequiredParameterException("Required dish data.");
            ////}
            ////if (string.IsNullOrWhiteSpace(dishRequest.Name))
            ////{
            ////    throw new RequiredParameterException("Name is required.");
            ////}
            ////if (dishRequest.Category == 0)
            ////{
            ////    throw new RequiredParameterException("Category is required.");
            ////}
            //if (dishRequest.Price <= 0)
            //{
            //    throw new InvalidateParameterException("Price must be greater than zero.");
            //}
            var categoryExists = await _CategoryExist.CategoryExist(dishRequest.Category);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {dishRequest.Category} not found.");
            }
            var createdDish = await _createDish.CreateDish(dishRequest);
            // if already exist a dish with that name, throw a 409 Conflict 
            if (createdDish == null)
            {
                throw new ConflictException("A dish with this name already exists.");
            }
            return CreatedAtAction(nameof(Search), new {id = createdDish.Id},createdDish);

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
            if(categoryId !=0  && categoryId != null)
            {
                var categoryExists = await _CategoryExist.CategoryExist(categoryId.Value);
                if (!categoryExists)
                {
                    throw new NotFoundException($"Category with ID {categoryId} not found.");
                }
            }

            if(orderPrice != null)
            {
                if(orderPrice != OrderPrice.ASC && orderPrice != OrderPrice.DESC)
                {
                    throw new OrderPriceException("Invalid order. Use ASC or DESC.");
                }
            }
            var list = await _SearchAsync.SearchAsync(name, categoryId, orderPrice, onlyActive);
            if (list == null || !list.Any())
            {
                throw new NotFoundException("No dishes found matching the criteria.");
            }
            
            return Ok(list);
            
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
            //if (dishRequest == null)
            //{
            //    throw new RequiredParameterException("Required dish data.");
            //}
            //if (string.IsNullOrWhiteSpace(dishRequest.Name))
            //{
            //    throw new RequiredParameterException("Name is required.");
            //}
            //if (dishRequest.Category == 0)
            //{
            //    throw new RequiredParameterException("Category is required.");
            //}
            //if (dishRequest.Price <= 0)
            //{
            //    throw new InvalidateParameterException("Price must be greater than zero.");
            //}

            var categoryExists = await _CategoryExist.CategoryExist(dishRequest.Category);
            if (!categoryExists)
            {
                throw new NotFoundException($"Category with ID {dishRequest.Category} not found.");
            }
            var result = await _UpdateDish.UpdateDish(id, dishRequest);
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
    }
}
