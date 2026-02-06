using Application.Dtos.Request.Module;
using FluentValidation;

namespace Application.Validators
{
    public class ModuleValidator : AbstractValidator<ModuleRequestDto>
    {
        public ModuleValidator()
        {
            RuleFor(x => x.ModuleName)
                .NotEmpty().WithMessage("El nombre de módul es requerido!")
                .MaximumLength(25).WithMessage("El nombre de módul no puede tener más de 25 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");
        }
    }
}
