using Application.Dtos.Request.InventoryPeriod;
using FluentValidation;

namespace Application.Validators.InventoryPeriod
{
    public class InventoryPeriodOpenValidator : AbstractValidator<InventoryPeriodOpenRequestDto>
    {
        public InventoryPeriodOpenValidator()
        {
            RuleFor(x => x.PeriodName)
                .NotEmpty().WithMessage("El nombre del período es requerido")
                .MaximumLength(30).WithMessage("El nombre del período no puede exceder 30 caracteres");

            RuleFor(x => x.StartDate)
                .NotNull().WithMessage("La fecha de inicio es requerida");

            RuleFor(x => x.EndDate)
                .NotNull().WithMessage("La fecha de fin es requerida")
                .GreaterThan(x => x.StartDate).WithMessage("La fecha de fin debe ser mayor a la fecha de inicio");
        }
    }
}
