using Organizas.Dtos.Response.ShoppingListItem;

namespace Organizas.Dtos.Response.ShoppingList
{
    public sealed record ShoppingListResponseDto(
        int Id,
        string Name,
        DateOnly? PurchaseDate
    );

    public sealed record ShoppingListDetailResponseDto(
        int Id,
        string Name,
        DateOnly? PurchaseDate,
        IReadOnlyList<ShoppingListItemResponseDto> Items
    );
}
