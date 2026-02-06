using Application.Dtos.Request.GoodsIssue;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsIssueDetailsValidator : AbstractValidator<GoodsIssueDetailsRequestDto>
    {
        public GoodsIssueDetailsValidator()
        {
            RuleFor(x => x.Item)
                .NotEmpty().WithMessage("El número de ítem es requerido!")
                .GreaterThan(0).WithMessage("El ítem debe ser mayor a 0!");

            RuleFor(x => x.IdProduct)
                .NotEmpty().WithMessage("El producto es requerido!");

            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("La cantidad es requerida!")
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0!");

            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithMessage("El precio unitario es requerido!")
                .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo!");

            RuleFor(x => x.TotalPrice)
                .NotEmpty().WithMessage("El precio total es requerido!")
                .GreaterThanOrEqualTo(0).WithMessage("El precio total no puede ser negativo!");

            RuleFor(x => x)
                .Must(x => x.TotalPrice == x.Quantity * x.UnitPrice)
                .WithMessage("El precio total debe ser igual a cantidad × precio unitario!")
                .WithName("TotalPrice");
        }
    }
}
