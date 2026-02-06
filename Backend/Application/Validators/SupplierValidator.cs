using Application.Dtos.Request.Supplier;
using FluentValidation;

namespace Application.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierRequestDto>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("El nombre de empresa es requerido!")
                .MaximumLength(50).WithMessage("El campo nombre de empresa no puede tener más de 50 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Contact)
                .NotEmpty().WithMessage("El contacto es requerido!")
                .MaximumLength(30).WithMessage("El contacto no puede tener más de 30 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido!")
                .MaximumLength(50).WithMessage("El correo no puede tener más de 50 caracteres!")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

        }
    }
}
