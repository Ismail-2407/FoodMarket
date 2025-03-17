using Microsoft.AspNetCore.Identity;

namespace FoodMarket.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}