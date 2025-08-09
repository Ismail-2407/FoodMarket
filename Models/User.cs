using Microsoft.AspNetCore.Identity;

namespace FoodMarket.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string CustomUsername { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? ResetPasswordToken { get; set; }
    }
}