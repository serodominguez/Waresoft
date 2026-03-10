using Application.Dtos.Request.StoreInventory;
using FluentValidation;

namespace Application.Validators
{
    public class StoreInventoryValidator : AbstractValidator<StoreInventoryRequestDto>
    {
        public StoreInventoryValidator() 
        {
            RuleFor(x => x.IdProduct)
                .GreaterThan(0).WithMessage("El identificador del producto es requerido");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
                //.PrecisionScale(18, 2, false).WithMessage("El precio puede tener máximo 2 decimales");
        }
    }
}
