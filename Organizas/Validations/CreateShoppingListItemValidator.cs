using FluentValidation;
using Organizas.Entities.Dtos.Request;

namespace Organizas.Validations
{
    public class CreateShoppingListItemValidator : AbstractValidator<CreateShoppingListItemDto>
    {
        public CreateShoppingListItemValidator() {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0m)
                .WithMessage("Quantidade deve que ser maior que zero")
                .PrecisionScale(12, 3, true);

            RuleFor(x => x.Unit)
                .IsInEnum()
                .When(x => x.Unit.HasValue)
                .WithMessage("A unidade informada é inválida.");
        }
    }
}
