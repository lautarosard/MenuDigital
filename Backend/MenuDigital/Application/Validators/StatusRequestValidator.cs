using Application.Models.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class StatusRequestValidator : AbstractValidator<StatusRequest>
    {
        public StatusRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El Estado no puede estar vacío.")
                .MaximumLength(50).WithMessage("El Estado no puede tener más de 50 caracteres.");
        }
    }
}
