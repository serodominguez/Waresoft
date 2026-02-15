using Application.Dtos.Request.GoodsReceipt;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsReceiptDetailsValidator : AbstractValidator<GoodsReceiptDetailsRequestDto>
    {
        public GoodsReceiptDetailsValidator()
        {
            RuleFor(x => x.Item)
                .NotEmpty().WithMessage("El número de ítem es requerido!")
                .GreaterThan(0).WithMessage("El ítem debe ser mayor a 0!");

            RuleFor(x => x.IdProduct)
                .NotEmpty().WithMessage("El producto es requerido!");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("La cantidad es requerida!")
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0!");

            RuleFor(x => x.UnitCost)
                .NotNull().WithMessage("El costo unitario es requerido!")
                .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo!");

            RuleFor(x => x.TotalCost)
                .NotNull().WithMessage("El costo total es requerido!")
                .GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo!");

            //Validación custom: TotalCost debe ser igual a Quantity * UnitCost
            RuleFor(x => x)
                .Must(x => x.TotalCost == x.Quantity * x.UnitCost)
                .WithMessage("El costo total debe ser igual a cantidad × costo unitario!")
                .WithName("TotalCost");
        }
    }
}
