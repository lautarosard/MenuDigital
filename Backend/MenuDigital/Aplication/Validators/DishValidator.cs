using Application.Models.Response;
using Application.Models.Request;
using FluentValidation;

namespace Application.Validators
{
    public class DishRequestValidator : AbstractValidator<DishRequest>
    {
        public DishRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(255).WithMessage("El nombre no puede superar los 255 caracteres");
            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede superar los 255 caracteres");
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("La categoria es necesaria")
                .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida");
        }
    }
}
