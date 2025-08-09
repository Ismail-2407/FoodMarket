using FoodMarket.Models;
using System.Collections.Generic;

namespace FoodMarket.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user, IList<string> roles);
        string? GetUserIdFromToken(string token);
    }
}