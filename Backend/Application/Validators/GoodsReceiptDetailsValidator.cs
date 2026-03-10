using Application.Dtos.Request.GoodsReceipt;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsReceiptDetailsValidator : AbstractValidator<GoodsReceiptDetailsRequestDto>
    {
        public GoodsReceiptDetailsValidator()
        {
            RuleFor(x => x.Item)
                .GreaterThan(0).WithMessage("El ítem debe ser mayor a 0");

            RuleFor(x => x.IdProduct)
                .GreaterThan(0).WithMessage("El producto es requerido");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

            RuleFor(x => x.UnitCost)
                .GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo");

            RuleFor(x => x.TotalCost)
                .GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo");

        }
    }
}
