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
        private readonly IUpdateItemFromOrderUseCase _updateItemFromOrder;
        private readonly IUpdateOrderItemStatusUseCase _updateOrderItemStatus;
        public OrderController(ICreateOrderUseCase createOrderUseCase, 
            IGetOrderWithFilterUseCase getOrderWithFilterUseCase,
            IGetOrderByIdUseCase getOrderById,
            IUpdateItemFromOrderUseCase updateItemFromOrder,
            IUpdateOrderItemStatusUseCase updateOrderItemStatus)
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrderWithFilterUseCase = getOrderWithFilterUseCase;
            _getOrderById = getOrderById;
            _updateItemFromOrder = updateItemFromOrder;
            _updateOrderItemStatus = updateOrderItemStatus;
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
        public async Task<IActionResult> GetOrders([FromQuery] int? statusId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {    
            var result = await _getOrderWithFilterUseCase.GetOrderWithFilter(statusId, from, to);
            if (result == null || !result.Any())
            {
                throw new NotFoundException("No orders found with the specified filters.");
            }
            return Ok(result);
               
            
            //return BadRequest(new ApiError("An error occurred while processing the request."));
            
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
        public async Task<IActionResult> GetOrderById(long id)
        {
            var order = await _getOrderById.GetOrderById(id);
            return Ok(order);
        }
        // PUT to update order items
        /// <summary>
        /// Actualizar orden existente
        /// </summary>
        /// <remarks>
        /// Actualiza los items de una orden existente.
        /// </remarks>
        [HttpPut("{orderId}")]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderItems(long orderId, [FromBody] OrderUpdateRequest request)
        {
            var response = await _updateItemFromOrder.UpdateItemQuantity(orderId, request);
            return Ok(response);
        }
        // PATCH: api/v1/order/1001/item/1
        /// <summary>
        /// Actualizar estado de item individual
        /// </summary>
        /// <remarks>
        /// Actualiza el estado de un item específico dentro de una orden.
        /// </remarks>
        [HttpPatch("{orderId}/item/{itemId}")]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderItemStatus(long orderId, int itemId, [FromBody] OrderItemUpdateRequest request)
        {
            var response = await _updateOrderItemStatus.UpdateItemStatus(orderId, itemId, request);
            return Ok(response);
        }
    }   
}
