using FoodMarket.Data;
using FoodMarket.Models;
using FoodMarket.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string?> Register(string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return null;

            var user = new User
            {
                Email = email,
                UserName = email
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role != null)
            {
                _context.UserRoles.Add(new IdentityUserRole<int>
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
}
