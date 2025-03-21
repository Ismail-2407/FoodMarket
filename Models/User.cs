using Microsoft.AspNetCore.Identity;

namespace FoodMarket.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string CustomUsername { get; set; } = string.Empty;
            
    }
}