using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Fluent Validation automáticamente rellena el ModelState con los errores
        if (!context.ModelState.IsValid)
        {
            // Si hay errores, detenemos la ejecución y devolvemos un BadRequest con los errores.
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No necesitamos hacer nada aquí para la validación, pero este método está disponible.
    }
}