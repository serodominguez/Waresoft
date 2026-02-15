using Application.Dtos.Request.GoodsReceipt;
using FluentValidation;

namespace Application.Validators
{
    public class GoodsReceiptValidator : AbstractValidator<GoodsReceiptRequestDto>
    {
        public GoodsReceiptValidator() 
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido!")
                .MaximumLength(15).WithMessage("El tipo no puede tener más de 15 caracteres!")
                .Must(type => type == "Adquisición" || type == "Regularización")
                .WithMessage("El tipo debe ser 'Adquisición' o 'Regularización'!");

            RuleFor(x => x.DocumentDate)
                .NotEmpty().WithMessage("La fecha del documento es requerida!")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser futura!");

            RuleFor(x => x.DocumentType)
                .NotEmpty().WithMessage("El tipo de documento es requerido!")
                .MaximumLength(15).WithMessage("El tipo de documento no puede tener más de 15 caracteres!");

            //Validación condicional: DocumentNumber requerido solo para Adquisición
            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es requerido para tipo Adquisición!")
                .MaximumLength(30).WithMessage("El número de documento no puede tener más de 30 caracteres!")
                .When(x => x.Type == "Adquisición");

            RuleFor(x => x.TotalAmount)
                .NotNull().WithMessage("El monto total es requerido!");

            RuleFor(x => x.Annotations)
                .MaximumLength(80).WithMessage("Las anotaciones no pueden tener más de 80 caracteres!");

            RuleFor(x => x.IdSupplier)
                .NotEmpty().WithMessage("El proveedor es requerido!");

            RuleFor(x => x.IdStore)
                .NotEmpty().WithMessage("El almacén es requerido!");

            // ===== VALIDACIONES DEL DETALLE =====

            //La colección no puede estar vacía
            RuleFor(x => x.GoodsReceiptDetails)
                .NotEmpty().WithMessage("Debe agregar al menos un detalle de entrada!")
                .Must(details => details != null && details.Any())
                .WithMessage("La entrada debe tener al menos un producto!");

            //Validar cada elemento del detalle
            RuleForEach(x => x.GoodsReceiptDetails)
                .SetValidator(new GoodsReceiptDetailsValidator());

            //Validación custom: Items únicos y secuenciales
            RuleFor(x => x.GoodsReceiptDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var items = details.Select(d => d.Item).ToList();
                    //Verificar que no haya ítems duplicados
                    return items.Count == items.Distinct().Count();
                })
                .WithMessage("Los números de ítem deben ser únicos!");

            //Validación custom: Productos únicos
            RuleFor(x => x.GoodsReceiptDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var products = details.Select(d => d.IdProduct).ToList();
                    return products.Count == products.Distinct().Count();
                })
                .WithMessage("No se puede agregar el mismo producto más de una vez!");

            //Validación custom: TotalAmount debe coincidir con la suma de detalles
            RuleFor(x => x)
                .Must(x =>
                {
                    if (x.GoodsReceiptDetails == null || !x.GoodsReceiptDetails.Any())
                        return true;

                    var sumDetails = x.GoodsReceiptDetails.Sum(d => d.TotalCost);
                    return x.TotalAmount == sumDetails;
                })
                .WithMessage("El monto total debe ser igual a la suma de los costos de los detalles!")
                .WithName("TotalAmount");
        }
    }
}
