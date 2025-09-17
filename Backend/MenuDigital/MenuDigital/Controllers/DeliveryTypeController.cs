using Application.Interfaces.IDeliveryType;
using Application.Models.Response;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenuDigital.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class DeliveryTypeController : ControllerBase
    {
        private readonly IGetAllDeliveryAsyncUseCase _getallDeliverys;
        public DeliveryTypeController(IGetAllDeliveryAsyncUseCase getallDeliverys)
        {
            _getallDeliverys = getallDeliverys;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DeliveryTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllDeliveryTypes()
        {
            var deliveryTypes = await _getallDeliverys.GetAllAsync();
            return Ok(deliveryTypes);
        }
    }
}
