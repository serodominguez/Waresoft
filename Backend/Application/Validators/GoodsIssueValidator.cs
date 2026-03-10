using Application.Dtos.Request.GoodsIssue;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsIssueValidator : AbstractValidator<GoodsIssueRequestDto>
    {
        public GoodsIssueValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido")
                .MaximumLength(20).WithMessage("El tipo no puede tener más de 20 caracteres")
                .Must(type => type == "Consignación" || type == "Ajuste de inventario" || type == "Ajuste de kardex")
                .WithMessage("El tipo debe ser 'Consignación', 'Ajuste de inventario' o 'Ajuste de kardex'");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("El monto total no puede ser negativo");

            RuleFor(x => x.Annotations)
                .MaximumLength(80).WithMessage("Las anotaciones no pueden tener más de 80 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Annotations));

            RuleFor(x => x.IdUser)
                .GreaterThan(0).WithMessage("El identificador del usuario es requerido");

            RuleFor(x => x.IdStore)
                .GreaterThan(0).WithMessage("El identificador del establecimiento es requerido");

            RuleFor(x => x.GoodsIssueDetails)
                .NotEmpty().WithMessage("Debe agregar al menos un producto a la salida");

            RuleForEach(x => x.GoodsIssueDetails)
                .SetValidator(new GoodsIssueDetailsValidator());

            RuleFor(x => x.GoodsIssueDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var items = details.Select(d => d.Item).ToList();
                    return items.Count == items.Distinct().Count();
                })
                .WithMessage("Los números de ítem deben ser únicos");

            RuleFor(x => x.GoodsIssueDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var products = details.Select(d => d.IdProduct).ToList();
                    return products.Count == products.Distinct().Count();
                })
                .WithMessage("No se puede agregar el mismo producto más de una vez");

            RuleFor(x => x)
                .Must(x =>
                {
                    if (x.GoodsIssueDetails == null || !x.GoodsIssueDetails.Any())
                        return true;

                    var sumDetails = x.GoodsIssueDetails.Sum(d => d.TotalPrice);
                    return x.TotalAmount == sumDetails;
                })
                .WithMessage("El monto total debe ser igual a la suma de los precios de los detalles")
                .WithName("TotalAmount");
        }
    }
}
