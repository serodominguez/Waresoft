using Application.Dtos.Request.InventoryPeriod;
using FluentValidation;

namespace Application.Validators.InventoryPeriod
{
    public class InventoryPeriodCloseValidator : AbstractValidator<InventoryPeriodCloseRequestDto>
    {
        public InventoryPeriodCloseValidator()
        {
            RuleFor(x => x.IdPeriod)
                .GreaterThan(0).WithMessage("El identificador del período es requerido");

            RuleFor(x => x.PhysicalCounts)
                .NotNull().WithMessage("El listado de conteo físico es requerido");

            RuleForEach(x => x.PhysicalCounts).ChildRules(count =>
            {
                count.RuleFor(x => x.IdProduct)
                    .GreaterThan(0).WithMessage("El identificador del producto es requerido");

                count.RuleFor(x => x.PhysicalStock)
                    .GreaterThanOrEqualTo(0).When(x => x.PhysicalStock.HasValue)
                    .WithMessage("El stock físico no puede ser negativo");
            });
        }
    }
}
