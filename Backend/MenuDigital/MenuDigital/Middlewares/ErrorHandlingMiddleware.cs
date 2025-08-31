using MenuDigital.Exceptions;
using MenuDigital.Exeptions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MenuDigital.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Continue processing the request
                await _next(context);
            }
            catch(RequiredParameterException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (InvalidParameterException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (OrderPriceException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
            }
            
        }
    }
}
