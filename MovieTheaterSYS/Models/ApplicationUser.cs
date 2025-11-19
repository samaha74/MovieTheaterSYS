using Microsoft.AspNetCore.Identity;

namespace MovieTheaterSYS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }

    }
}
