using Application.Dtos.Request.GoodsIssue;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsIssueValidator : AbstractValidator<GoodsIssueRequestDto>
    {
        public GoodsIssueValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido!")
                .MaximumLength(15).WithMessage("El tipo no puede tener más de 15 caracteres!")
                .Must(type => type == "CONSIGNACIÓN" || type == "REGULARIZACIÓN")
                .WithMessage("El tipo debe ser 'CONSIGNACIÓN' o 'REGULARIZACIÓN'!");

            RuleFor(x => x.TotalAmount)
                .NotEmpty().WithMessage("El monto total es requerido!");

            RuleFor(x => x.Annotations)
                .MaximumLength(80).WithMessage("Las anotaciones no pueden tener más de 80 caracteres!");

            RuleFor(x => x.IdUser)
                .NotEmpty().WithMessage("El usuario es requerido!");

            RuleFor(x => x.IdStore)
                .NotEmpty().WithMessage("El almacén es requerido!");

            RuleFor(x => x.GoodsIssueDetails)
                .NotEmpty().WithMessage("Debe agregar al menos un detalle de salida!")
                .Must(details => details != null && details.Any())
                .WithMessage("La salida debe tener al menos un producto!");

            RuleForEach(x => x.GoodsIssueDetails)
                .SetValidator(new GoodsIssueDetailsValidator());

            RuleFor(x => x.GoodsIssueDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var items = details.Select(d => d.Item).ToList();
                    return items.Count == items.Distinct().Count();
                })
                .WithMessage("Los números de ítem deben ser únicos!");

            RuleFor(x => x.GoodsIssueDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var products = details.Select(d => d.IdProduct).ToList();
                    return products.Count == products.Distinct().Count();
                })
                .WithMessage("No se puede agregar el mismo producto más de una vez!");

            RuleFor(x => x)
                .Must(x =>
                {
                    if (x.GoodsIssueDetails == null || !x.GoodsIssueDetails.Any())
                        return true;

                    var sumDetails = x.GoodsIssueDetails.Sum(d => d.TotalPrice);
                    return x.TotalAmount == sumDetails;
                })
                .WithMessage("El monto total debe ser igual a la suma de los precios de los detalles!")
                .WithName("TotalAmount");
        }
    }
}
