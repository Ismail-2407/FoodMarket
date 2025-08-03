using Microsoft.AspNetCore.Identity;

namespace FoodMarket.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string CustomUsername { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? ResetPasswordToken { get; set; }

        // public UserRole Role { get; set; } = UserRole.User;
    }

    // public enum UserRole
    // {
    //     Admin,
    //     User
    // }
}