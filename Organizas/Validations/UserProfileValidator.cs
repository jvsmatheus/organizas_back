using FluentValidation;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Request.UserProfile;

namespace Organizas.Validations
{
    public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome do perfil é obrigatório")
                .MaximumLength(150)
                .WithMessage("Nome tem um tamanho máximo de 150 caracteres");
        }
    }
}
