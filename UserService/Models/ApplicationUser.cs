using Microsoft.AspNetCore.Identity;

namespace UserService.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Role { get; set; }
    }
}
