using Application.Dtos.Request.Role;
using FluentValidation;

namespace Application.Validators
{
    public class RoleValidator : AbstractValidator<RoleRequestDto>
    {
        public RoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("El nombre de rol es requerido!")
                .MaximumLength(20).WithMessage("El nombre de rol no puede tener más de 20 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");
        }
    }
}
