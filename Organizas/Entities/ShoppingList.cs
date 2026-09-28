namespace Organizas.Entities
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? PurchaseDate { get; set; }
        public List<ShoppingListItem> Items { get; set; } = [];
        public string UserProfileId { get; set; }

        public UserProfile UserProfile { get; set; }
    }
}
