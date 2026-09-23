namespace Organizas.Entities
{
    public class UserProfile
    {
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}
