using FluentValidation;
using Organizas.Dtos.Request.ShoppingListItem;

namespace Organizas.Validations
{
    public class CreateShoppingListItemValidator : AbstractValidator<CreateShoppingListItemDto>
    {
        public CreateShoppingListItemValidator() {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome do item é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0m)
                .WithMessage("Quantidade do item deve que ser maior que zero")
                .PrecisionScale(12, 3, true);

            RuleFor(x => x.Unit)
                .IsInEnum()
                .When(x => x.Unit.HasValue)
                .WithMessage("A unidade do item informada é inválida.");
        }
    }

    public class UpdateShoppingListItemValidator : AbstractValidator<UpdateShoppingListItemDto>
    {
        public UpdateShoppingListItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome do item é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0m)
                .WithMessage("Quantidade do item deve que ser maior que zero")
                .PrecisionScale(12, 3, true);

            RuleFor(x => x.Unit)
                .IsInEnum()
                .When(x => x.Unit.HasValue)
                .WithMessage("A unidade do item informada é inválida.");
        }
    }
}
