using Application.Models.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class ItemsValidator : AbstractValidator<Items>
    {
        public ItemsValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("El ID del ítem no puede estar vacío.");
            RuleFor(x => x.quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            RuleFor(x => x.notes)
                .MaximumLength(250).WithMessage("Las notas no pueden exceder los 250 caracteres.");
        }
    }
}
