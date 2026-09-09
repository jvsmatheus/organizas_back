namespace Organizas.Exceptions
{
    public sealed class ItemNotFoundException : Exception
    {
        public int ItemId { get; }

        public ItemNotFoundException() : base("Item não encontrado") { }
    }
}
