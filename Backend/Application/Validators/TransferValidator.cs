using Application.Dtos.Request.Transfer;
using FluentValidation;

namespace Application.Validators
{
    public class TransferValidator : AbstractValidator<TransferRequestDto>
    {
        public TransferValidator()
        {

            RuleFor(x => x.TotalAmount)
                .NotNull().WithMessage("El monto total es requerido!");

            RuleFor(x => x.Annotations)
                .MaximumLength(80).WithMessage("Las anotaciones no pueden tener más de 80 caracteres!");

            RuleFor(x => x.IdStoreOrigin)
                .NotEmpty().WithMessage("El almacén de origen es requerido!");

            RuleFor(x => x.IdStoreDestination)
                .NotEmpty().WithMessage("El almacén de destino es requerido!");

            RuleFor(x => x.TransferDetails)
                .NotEmpty().WithMessage("Debe agregar al menos un detalle de traspaso!")
                .Must(details => details != null && details.Any())
                .WithMessage("El traspaso debe tener al menos un producto!");

            RuleForEach(x => x.TransferDetails)
                .SetValidator(new TransferDetailsValidator());

            RuleFor(x => x.TransferDetails)
                .Must(details =>
                {
                    if (details == null || !details.Any()) return true;

                    var items = details.Select(d => d.Item).ToList();
                    return items.Count == items.Distinct().Count();
                })
                .WithMessage("Los números de ítem deben ser únicos!");

            RuleFor(x => x.TransferDetails)
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
                    if (x.TransferDetails == null || !x.TransferDetails.Any())
                        return true;

                    var sumDetails = x.TransferDetails.Sum(d => d.TotalPrice);
                    return x.TotalAmount == sumDetails;
                })
                .WithMessage("El monto total debe ser igual a la suma de los precios de los detalles!")
                .WithName("TotalAmount");
        }
    }
}
