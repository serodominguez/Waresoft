using Application.Dtos.Request.Brand;
using FluentValidation;

namespace Application.Validators
{
    public class BrandValidator : AbstractValidator<BrandRequestDto>
    {
        public BrandValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty().WithMessage("El nombre de marca es requerido!")
                .MaximumLength(25).WithMessage("El nombre de marca no puede tener más de 25 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

        }
    }
}
