using Application.Dtos.Request.Store;
using FluentValidation;

namespace Application.Validators
{
    public class StoreValidator : AbstractValidator<StoreRequestDto>
    {
        public StoreValidator()
        {
            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage("El nombre del establecimiento es requerido")
                .MaximumLength(50).WithMessage("El nombre del establecimiento no puede tener más de 50 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Manager)
                .NotEmpty().WithMessage("El encargado es requerido")
                .MaximumLength(30).WithMessage("El encargado no puede tener más de 30 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es requerida")
                .MaximumLength(60).WithMessage("La dirección no puede tener más de 60 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("La ciudad es requerida")
                .MaximumLength(15).WithMessage("La ciudad no puede tener más de 15 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido")
                .MaximumLength(50).WithMessage("El correo no puede tener más de 50 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Length(8).WithMessage("El número de teléfono debe tener 8 dígitos")
                .Matches(@"^\d+$").WithMessage("El número de teléfono solo debe contener dígitos")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.ProfitMargin)
                .NotNull().WithMessage("El margen de ganancia es requerido")
                .InclusiveBetween(0.10m, 0.95m).WithMessage("El margen de ganancia debe estar entre 0.10 y 0.95");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido")
                .MaximumLength(15).WithMessage("El tipo no puede tener más de 15 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");
        }
    }
}
