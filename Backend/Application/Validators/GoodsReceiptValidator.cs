using Application.Dtos.Request.GoodsReceipt;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsReceiptValidator : AbstractValidator<GoodsReceiptRequestDto>
    {
        public GoodsReceiptValidator() 
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido")
                .MaximumLength(20).WithMessage("El tipo no puede tener más de 20 caracteres")
                .Must(type => type == "Adquisición" || type == "Alta" || type == "Ajuste de inventario" || type == "Ajuste de kardex")
                .WithMessage("El tipo debe ser: 'Adquisición', 'Alta', 'Ajuste de inventario' o 'Ajuste de kardex'");

            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("El tipo de documento es requerido para tipo Adquisición")
                .MaximumLength(15).WithMessage("El tipo de documento no puede tener más de 15 caracteres")
                .When(x => x.Type == "Adquisición");

            RuleFor(x => x.DocumentDate)
                .NotEmpty().WithMessage("La fecha del documento es requerida para tipo Adquisición")
                .When(x => x.Type == "Adquisición");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es requerido para tipo Adquisición")
                .MaximumLength(30).WithMessage("El número de documento no puede tener más de 30 caracteres")
                .When(x => x.Type == "Adquisición");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("El monto total no puede ser negativo");

            RuleFor(x => x.Annotations)
                .MaximumLength(80).WithMessage("Las anotaciones no pueden tener más de 80 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Annotations));

            RuleFor(x => x.IdSupplier)
                .GreaterThan(0).WithMessage("El identificador del proveedor es requerido")
                .When(x => x.Type == "Adquisición");

            RuleFor(x => x.IdStore)
                .GreaterThan(0).WithMessage("El identificador del establecimiento es requerido");

            RuleFor(x => x.GoodsReceiptDetails)
                .NotEmpty().WithMessage("Debe agregar al menos un producto a la entrada");

            RuleForEach(x => x.GoodsReceiptDetails)
                .SetValidator(new GoodsReceiptDetailsValidator());

            RuleFor(x => x.GoodsReceiptDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var items = details.Select(d => d.Item).ToList();

                    return items.Count == items.Distinct().Count();
                })
                .WithMessage("Los números de ítem deben ser únicos");

            RuleFor(x => x.GoodsReceiptDetails)
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
                    if (x.GoodsReceiptDetails == null || !x.GoodsReceiptDetails.Any())
                        return true;

                    var sumDetails = x.GoodsReceiptDetails.Sum(d => d.TotalCost);
                    return x.TotalAmount == sumDetails;
                })
                .WithMessage("El monto total debe ser igual a la suma de los costos de los detalles")
                .WithName("TotalAmount");
        }
    }
}
