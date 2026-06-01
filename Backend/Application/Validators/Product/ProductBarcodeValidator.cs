using Application.Dtos.Request.Product;
using FluentValidation;

namespace Application.Validators.Product
{
    public class ProductBarcodeValidator : AbstractValidator<ProductBarcodeRequestDto>
    {
        public ProductBarcodeValidator() 
        {

            RuleFor(x => x.IdProduct)
            .GreaterThan(0).WithMessage("El identificador del producto es requerido");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 200).WithMessage("La cantidad debe estar entre 1 y 200.");
        }
    }
}
