using FluentValidation;
using migrapp_api.DTOs.Admin;

namespace migrapp_api.Validators.Admin
{
    public class CreateUserByAdminDtoValidator : AbstractValidator<CreateUserByAdminDto>
    {
        public CreateUserByAdminDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).MaximumLength(100);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Phone).MaximumLength(20);
            RuleFor(x => x.PhonePrefix).MaximumLength(10);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");
            RuleFor(x => x.UserType)
                .NotEmpty()
                .Must(x => new[] { "admin", "lawyer", "auditor", "reader" }.Contains(x))
                .WithMessage("Tipo de usuario inválido");
        }
    }
}
