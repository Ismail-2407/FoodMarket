using FoodMarket.Models;

namespace FoodMarket.Services;

public interface IUserService
{
    Task<string?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string password);
    Task<(string Role, bool Found)> GetRoleByIdAsync(int userId);
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdatePasswordAsync(int id, string newPassword);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UpdateAsync(User user);
    Task EnsureAdminUserAsync(); 
    Task<(string Token, string Role)?> LoginWithRoleAsync(string email, string password);
}