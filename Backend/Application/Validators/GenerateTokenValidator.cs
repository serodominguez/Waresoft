using Application.Dtos.Request.User;
using FluentValidation;

namespace Application.Validators
{
    public class GenerateTokenValidator : AbstractValidator<TokenRequestDto>
    {
        public GenerateTokenValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es requerido")
                .MaximumLength(20).WithMessage("El nombre de usuario no puede tener más de 20 caracteres")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$")
                .WithMessage("El nombre de usuario contiene caracteres invalidos");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida")
                .MinimumLength(1).WithMessage("La contraseña es demasiado corta")
                .MaximumLength(100).WithMessage("La contraseña es demasiado larga");
        }
    }
}
