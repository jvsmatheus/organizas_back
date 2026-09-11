namespace Organizas.Exceptions
{
    public sealed class ItemNotFoundException : Exception
    {
        public int ItemId { get; }

        public ItemNotFoundException() : base() { }

        public ItemNotFoundException(string message) : base(message) { }
    }
}
