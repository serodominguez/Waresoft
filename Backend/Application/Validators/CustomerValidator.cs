using Application.Dtos.Request.Customer;
using FluentValidation;

namespace Application.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerRequestDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Names)
                .NotEmpty().WithMessage("El nombre es requerido!")
                .MaximumLength(30).WithMessage("El nombre no puede tener más de 30 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.LastNames)
                .NotEmpty().WithMessage("Los apellidos son requeridos!")
                .MaximumLength(50).WithMessage("Los apellidos no pueden tener más de 50 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.IdentificationNumber)
                .MaximumLength(10).WithMessage("El número de identificación no puede tener más de 10 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.IdentificationNumber));
        }
    }
}
