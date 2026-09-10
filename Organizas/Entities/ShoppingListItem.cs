using Organizas.Enum;

namespace Organizas.Entities
{
    public sealed class ShoppingListItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Quantity { get; set; }
        public ShoppingListItemUnitEnum? Unit { get; set; }
        public bool IsChecked { get; set; }
        public int ShoppingListId { get; set; }
    }
}
