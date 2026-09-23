using Microsoft.AspNetCore.Identity;

namespace Organizas.Entities
{
    public class User : IdentityUser
    {
        public UserProfile? Profile { get; set; }
    }
}
