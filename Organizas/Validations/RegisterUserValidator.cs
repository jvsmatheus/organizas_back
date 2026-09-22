using FluentValidation;
using Organizas.Dtos.Request.Auth;

namespace Organizas.Validations
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-mail é obrigatório")
                .EmailAddress()
                .WithMessage("E-mail inválido")
                .Matches(@"^[^@\s]+@[^@\s.]+(?:\.[^@\s.]+)+$")
                .WithMessage("Informe um e-mail com domínio completo, como nome@exemplo.com.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Senha é obrigatório");
        }
    }
}
