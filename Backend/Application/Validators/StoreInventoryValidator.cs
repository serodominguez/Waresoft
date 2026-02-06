using Application.Dtos.Request.StoreInventory;
using FluentValidation;

namespace Application.Validators
{
    public class StoreInventoryValidator : AbstractValidator<StoreInventoryRequestDto>
    {
        public StoreInventoryValidator() 
        {
            RuleFor(x => x.IdProduct)
                .NotEmpty().WithMessage("El Id del producto es requerido")
                .GreaterThan(0).WithMessage("El Id del producto debe ser mayor a 0");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("El precio es requerido")
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");
                //.PrecisionScale(18, 2, false).WithMessage("El precio puede tener máximo 2 decimales");
        }
    }
}
