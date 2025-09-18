using Application.Exceptions;
using Application.Interfaces.IOrder;
using Application.Models.Request;
using Application.Models.Response;
using Application.Models.Response.Dish;
using Application.Models.Response.Order;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MenuDigital.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class OrderController : ControllerBase
    {
        private readonly ICreateOrderUseCase _createOrderUseCase;
        private readonly IGetOrderWithFilterUseCase _getOrderWithFilterUseCase;
        private readonly IGetOrderByIdUseCase _getOrderById;
        private readonly IUpdateItemFromOrder _updateItemFromOrder;
        public OrderController(ICreateOrderUseCase createOrderUseCase, 
            IGetOrderWithFilterUseCase getOrderWithFilterUseCase,
            IGetOrderByIdUseCase getOrderById,
            IUpdateItemFromOrder updateItemFromOrder)
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrderWithFilterUseCase = getOrderWithFilterUseCase;
            _getOrderById = getOrderById;
            _updateItemFromOrder = updateItemFromOrder;
        }

        // POST
        /// <summary>
        /// Crear nueva orden.
        /// </summary>
        /// <remarks>
        /// Crea un nueva orden en el menú del restaurante.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(OrderCreateResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] Application.Models.Request.OrderRequest orderRequest)
        {
            try
            {
                var result = await _createOrderUseCase.CreateOrder(orderRequest);

                return CreatedAtAction(nameof(CreateOrder), new { id = result.orderNumber }, result);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ApiError("An error occurred while processing the request."));
            }
        }

        // GET with filters
        /// <summary>
        /// Buscar órdenes.
        /// </summary>
        /// <remarks>
        /// Obtiene una lista de órdenes con filtros opcionales.
        /// </remarks>
        //("search")
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDetailsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrders([FromQuery] int? statusId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            //sacar try
            try
            {
                var result = await _getOrderWithFilterUseCase.GetOrderWithFilter(statusId, from, to);
                if (result == null || !result.Any())
                {
                    return NotFound(new ApiError("No orders found with the specified filters."));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ApiError("An error occurred while processing the request."));
            }
        }
        //GET by ID
        /// <summary>
        /// Obtiene una order por su ID.
        /// </summary>
        /// <remarks>
        /// Busca un order específico en el menú usando su identificador único.
        /// </remarks>
        [HttpGet("{id}")]
        [SwaggerOperation(
        Summary = "Buscar orders por ID",
        Description = "Buscar orders por ID."
        )]
        [ProducesResponseType(typeof(DishResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        private async Task<IActionResult> GetDishById(long id)
        {
            var dish = await _getOrderById.GetOrderById(id);
            if (dish == null)
            {
                throw new NotFoundException($"Order with ID {id} not found.");
            }
            return Ok(dish);
        }
        // PUT to update order items
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrderItemStatus(long orderId, [FromBody] OrderUpdateRequest request)
        {
            var response = await _updateItemFromOrder.UpdateItemQuantity(orderId, request);
            return Ok(response);
        }
        // PATCH: api/v1/order/1001/item/1
        [HttpPatch("{orderId}/item/{itemId}")]
        //aplicar
    }   
}
