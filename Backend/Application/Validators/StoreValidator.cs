using Application.Dtos.Request.Store;
using FluentValidation;

namespace Application.Validators
{
    public class StoreValidator : AbstractValidator<StoreRequestDto>
    {
        public StoreValidator()
        {
            RuleFor(x => x.StoreName)
                .NotEmpty().WithMessage("El nombre de tienda es requerido!")
                .MaximumLength(50).WithMessage("El nombre de tienda no puede tener más de 50 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Manager)
                .NotEmpty().WithMessage("El encargado es requerido!")
                .MaximumLength(30).WithMessage("El encargado no puede tener más de 30 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("La dirección es requerida!")
                .MaximumLength(60).WithMessage("La dirección no puede tener más de 60 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("La ciudad es requerida!")
                .MaximumLength(15).WithMessage("La ciudad no puede tener más de 15 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido!")
                .MaximumLength(50).WithMessage("El correo no puede tener más de 50 caracteres!")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido!")
                .MaximumLength(15).WithMessage("El tipo no puede tener más de 15 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");
        }
    }
}
