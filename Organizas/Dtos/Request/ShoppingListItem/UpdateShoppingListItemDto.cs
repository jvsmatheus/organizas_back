using Organizas.Enum;

namespace Organizas.Dtos.Request.ShoppingListItem
{
    public record UpdateShoppingListItemDto(
        string Name,
        decimal Quantity,
        ShoppingListItemUnitEnum? Unit,
        bool IsChecked,
        int ShoppingListId
    );
}
