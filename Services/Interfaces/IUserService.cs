using FoodMarket.Models;

namespace FoodMarket.Services;

public interface IUserService
{
    Task<string?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string password);
    Task<(string Role, bool Found)> GetRoleByIdAsync(string userId);
    Task<User?> GetByIdAsync(string id);
    Task<List<User>> GetAllAsync();
    Task<bool> DeleteAsync(string id);
    Task<bool> UpdatePasswordAsync(string id, string newPassword);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UpdateAsync(User user);
    Task EnsureAdminUserAsync(); 
    Task<(string Token, string Role)?> LoginWithRoleAsync(string email, string password);


}