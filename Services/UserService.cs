using System.Security.Claims;
using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public UserService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return null;

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
            .ToListAsync();

        return _tokenService.CreateToken(user, roles);
    }

    public async Task<(string Token, string Role)?> LoginWithRoleAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return null;

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
            .ToListAsync();

        var token = _tokenService.CreateToken(user, roles);
        var mainRole = roles.FirstOrDefault() ?? "User";

        return (token, mainRole);
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return false;

        var passwordHasher = new PasswordHasher<User>();
        var user = new User
        {
            Email = email,
            UserName = email,
            PasswordHash = passwordHasher.HashPassword(null!, password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (role != null)
        {
            _context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id
            });
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<(string Role, bool Found)> GetRoleByIdAsync(string userId)
    {
        var role = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
            .FirstOrDefaultAsync();

        return role != null ? (role, true) : ("", false);
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(string id, string newPassword)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(null!, newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task EnsureAdminUserAsync()
    {
        var email = "ismailqasimov71@gmail.com";
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            var passwordHasher = new PasswordHasher<User>();
            user = new User
            {
                Email = email,
                UserName = email,
                PasswordHash = passwordHasher.HashPassword(null!, "Admin@12345")
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (role == null)
        {
            role = new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        var hasRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
        if (!hasRole)
        {
            _context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id
            });
            await _context.SaveChangesAsync();
        }

        Console.WriteLine($"[✔] Админ-пользователь создан: {email}");
    }
}
