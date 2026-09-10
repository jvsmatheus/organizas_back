using FluentValidation;
using Organizas.Dtos.Request.ShoppingList;

namespace Organizas.Validations
{
    public class CreateShoppingListValidator : AbstractValidator<CreateShoppingListDto>
    {
        public CreateShoppingListValidator() {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome da lista é obrigatório");
        }
    }

    public class UpdateShoppingListValidator : AbstractValidator<UpdateShoppingListDto>
    {
        public UpdateShoppingListValidator() {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome da lista é obrigatório");
        }
    }
}
