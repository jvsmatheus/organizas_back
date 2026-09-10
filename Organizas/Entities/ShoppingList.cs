namespace Organizas.Entities
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ShoppingListItem> Items { get; set; } = [];
    }
}
