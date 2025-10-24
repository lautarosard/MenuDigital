using Application.Models.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class DeliveryValidator : AbstractValidator<Delivery>
    {
        public DeliveryValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("El ID del ítem no puede estar vacío.");
            // Validar el campo 'to' solo si el tipo de entrega es Delivery
            When(x => x.id == 1, () =>
            {
                RuleFor(x => x.to)
                    .NotEmpty().WithMessage("La dirección de entrega no puede estar vacía para entregas a domicilio.")
                    .MaximumLength(250).WithMessage("La dirección no puede exceder los 250 caracteres.");
            });
            // Opcional: permitir vacío para otros tipos
            When(x => x.id != 1, () =>
            {
                RuleFor(x => x.to)
                    .MaximumLength(250).WithMessage("El campo 'to' no puede exceder los 250 caracteres si se completa.");
            });

        }
    }
}
