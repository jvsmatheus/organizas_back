using Organizas.Enum;

namespace Organizas.Dtos.Request
{
    public record CreateShoppingListItemDto(
        string Name,
        decimal Quantity,
        ShoppingListItemUnitEnum? Unit
    );
}
