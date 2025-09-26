using Application.Models.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator() 
        {
            RuleFor(x => x.items)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .ForEach(i => i.SetValidator(new ItemsValidator()));

            RuleFor(o => o.delivery)
            .NotNull().WithMessage("Delivery information is required.")
            .SetValidator(new DeliveryValidator()!);

            RuleFor(o => o.notes)
            .MaximumLength(250).WithMessage("Notes cannot exceed 250 characters.");

        }
    }
}
