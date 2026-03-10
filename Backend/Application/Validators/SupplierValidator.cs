using Application.Dtos.Request.Supplier;
using FluentValidation;

namespace Application.Validators
{
    public class SupplierValidator : AbstractValidator<SupplierRequestDto>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("El nombre de la empresa es requerido")
                .MaximumLength(50).WithMessage("El campo nombre de empresa no puede tener más de 50 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Contact)
                .NotEmpty().WithMessage("El contacto es requerido")
                .MaximumLength(30).WithMessage("El contacto no puede tener más de 30 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.PhoneNumber)
                .GreaterThan(0).WithMessage("El número de teléfono debe ser positivo")
                .Must(HaveMaximum8Digits).WithMessage("El número de teléfono no puede tener más de 8 dígitos")
                .When(x => x.PhoneNumber.HasValue);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido")
                .MaximumLength(50).WithMessage("El correo no puede tener más de 50 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

        }

        private bool HaveMaximum8Digits(int? phoneNumber)
        {
            if (!phoneNumber.HasValue) return true;

            var digits = phoneNumber.Value.ToString().Length;
            return digits <= 8;
        }
    }
}
