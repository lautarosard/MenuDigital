using Application.Models.Response;
using MenuDigital.Exceptions;
using MenuDigital.Exeptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MenuDigital.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //Tomorrow search if this is okay -REMINDER
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Continue processing the request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError(ex, $"Catch Error: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
            
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //This method select the server response and wrapp with the ApiError

            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            string message = ex.Message;

            switch(ex)
            {
                case RequiredParameterException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case InvalidParameterException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case ConflictException:
                    statusCode = HttpStatusCode.Conflict;
                    break;
                case OrderPriceException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Internal Server Error";
                    break;
                
                
            }
            context.Response.StatusCode = (int)statusCode;

            var error = new ApiError(message);
            var result = JsonSerializer.Serialize(error);

            await context.Response.WriteAsync(result);
        }
    }
}
