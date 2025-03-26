using FluentValidation;
using migrapp_api.DTOs.Auth;

namespace migrapp_api.Validators.Auth
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio")
                .EmailAddress().WithMessage("Correo inválido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria");

            RuleFor(x => x.PreferredMfaMethod)
                .Must(x => x == "email" || x == "sms")
                .WithMessage("Método MFA inválido. Debe ser 'email' o 'sms'");
        }
    }
}

