using Application.Dtos.Request.Category;
using FluentValidation;

namespace Application.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryRequestDto>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("El nombre de categoría es requerido!")
                .MaximumLength(25).WithMessage("El nombre de categoría no puede tener más de 25 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Description)
                .MaximumLength(50).WithMessage("La descripción no puede tener más de 50 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");
        }
    }
}
