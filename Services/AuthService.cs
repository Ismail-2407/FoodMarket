using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string?> Register(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return null;

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

        return await GenerateJwtToken(user);
    }

    public async Task<string?> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed) return null;

        return await GenerateJwtToken(user);
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
            .ToListAsync();

        return _tokenService.CreateToken(user, roles);
    }
}
