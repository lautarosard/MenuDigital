using Application.Models.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class OrderUpdateValidator : AbstractValidator<OrderUpdateRequest>
    {
        public OrderUpdateValidator() 
        {
            RuleFor(x => x.items)
            .NotEmpty().WithMessage("Order must contain at least one item.")
            .ForEach(i => i.SetValidator(new ItemsValidator()));
        }
    }
}
