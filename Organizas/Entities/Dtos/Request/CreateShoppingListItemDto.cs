using Organizas.Enum;

namespace Organizas.Entities.Dtos.Request
{
    public record CreateShoppingListItemDto(
        string Name,
        decimal Quantity,
        ShoppingListItemUnitEnum? Unit
    );
}
