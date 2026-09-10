using Organizas.Enum;

namespace Organizas.Dtos.Request.ShoppingListItem
{
    public record CreateShoppingListItemDto(
        string Name,
        decimal Quantity,
        ShoppingListItemUnitEnum? Unit,
        int ShoppingListId
    );
}
