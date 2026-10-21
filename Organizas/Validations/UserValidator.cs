using FluentValidation;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Request.User;
using Organizas.Dtos.Request.UserProfile;

namespace Organizas.Validations
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserRequestDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-mail é obrigatório")
                .EmailAddress()
                .WithMessage("E-mail inválido");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Senha é obrigatório")
                .MinimumLength(10)
                .WithMessage("Senha tem um tamanho mínimo de 10 caracteres")
                .MaximumLength(255)
                .WithMessage("Senha tem um tamanho máximo de 255 caracteres")
                .Matches(@"\d")
                .WithMessage("Senha deve conter pelo menos um número")
                .Matches(@"[^a-zA-Z0-9]")
                .WithMessage("Senha deve conter pelo menos um caractere não alfanumérico");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Primeiro nome do perfil é obrigatório")
                .MaximumLength(150)
                .WithMessage("Nome tem um tamanho máximo de 150 caracteres");

            RuleFor(x => x.LastName)
                .MaximumLength(150)
                .WithMessage("Nome tem um tamanho máximo de 150 caracteres");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(11)
                .WithMessage("Número de telefone pode ter no máximo 11 dígitos");
        }
    }
}