using Application.Interfaces.IStatus;
using Application.Models.Response;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MenuDigital.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class StatusController : ControllerBase
    {
        private readonly IGetAllStatusAsyncUseCase _getAllStatus;
        public StatusController(IGetAllStatusAsyncUseCase getAllStatus)
        {
            _getAllStatus = getAllStatus;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllStatuses()
        {
            var statuses = await _getAllStatus.GetAllStatuses();
            return Ok(statuses);
        }

    }
}
