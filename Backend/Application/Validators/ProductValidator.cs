using Application.Dtos.Request.Product;
using FluentValidation;

namespace Application.Validators
{
    public class ProductValidator : AbstractValidator<ProductRequestDto>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Code)
                .MaximumLength(25).WithMessage("El código no puede tener más de 25 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida!")
                .MaximumLength(50).WithMessage("La descripción no puede tener más de 50 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Material)
                .MaximumLength(25).WithMessage("El material no puede tener más de 25 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Color)
                .MaximumLength(20).WithMessage("El color no puede tener más de 20 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.UnitMeasure)
                .NotEmpty().WithMessage("La unidad de medida es requerida!")
                .MaximumLength(15).WithMessage("La unidad de medida no puede tener más de 15 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.IdBrand)
                .NotNull().WithMessage("El identificador de la marca no puede ser nulo!");

            RuleFor(x => x.IdCategory)
                .NotNull().WithMessage("El identificador de la categoría no puede ser nulo!");

        }
    }
}
