using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.IOrder;
using Application.Models.Response.Order;
using Application.Models.Response;

namespace MenuDigital.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class OrderController : ControllerBase
    {
        private readonly ICreateOrderUseCase _createOrderUseCase;
        public OrderController(ICreateOrderUseCase createOrderUseCase)
        {
            _createOrderUseCase = createOrderUseCase;
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

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ApiError("An error occurred while processing the request." ));
            }
        }
    }
}
