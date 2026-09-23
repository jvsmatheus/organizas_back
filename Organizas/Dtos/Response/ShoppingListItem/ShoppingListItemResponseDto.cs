using Organizas.Enum;

namespace Organizas.Dtos.Response.ShoppingListItem
{
    public sealed record ShoppingListItemResponseDto(
        int Id,
        string Name,
        decimal Quantity,
        decimal? EstimatedUnitPrice,
        ShoppingListItemUnitEnum? Unit,
        bool IsChecked
    );
}
