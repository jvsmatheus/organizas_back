using Organizas.Enum;

namespace Organizas.Dtos.Request
{
    public record UpdateShoppingListItemDto(
        string Name,
        decimal Quantity,
        ShoppingListItemUnitEnum? Unit,
        bool IsChecked
    );
}
