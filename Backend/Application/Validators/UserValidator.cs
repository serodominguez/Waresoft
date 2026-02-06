using Application.Dtos.Request.User;
using FluentValidation;

namespace Application.Validators
{
    public class UserValidator : AbstractValidator<UserRequestDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es requerido!")
                .MaximumLength(20).WithMessage("El campo nombre de usuario no puede tener más de 20 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida!");

            RuleFor(x => x.Names)
                .NotEmpty().WithMessage("El nombre es requerido!")
                .MaximumLength(30).WithMessage("El nombre no puede tener más de 30 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.LastNames)
                .NotEmpty().WithMessage("Los apellidos son requeridos!")
                .MaximumLength(50).WithMessage("Los apellidos no pueden tener más de 50 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$");

            RuleFor(x => x.IdentificationNumber)
                .MaximumLength(10).WithMessage("El número de identificación no puede tener más de 10 caracteres!")
                .Matches("^[a-zA-Z0-9 áéíóúñÁÉÍÓÚÑ]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.IdentificationNumber));

            RuleFor(x => x.IdRole)
                .NotNull().WithMessage("El identificador del rol no puede ser nulo!");

            RuleFor(x => x.IdStore)
                .NotNull().WithMessage("El identificador de la sucursal no puede ser nulo!");

        }
    }
}
