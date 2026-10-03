using System.Security.Claims;
using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.API.Controllers;

[ApiController]
[Route("api/favorites")]
//[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _db;

    public FavoritesController(AppDbContext db)
    {
        _db = db;
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("nameid")
                      ?? User.FindFirstValue("sub");

        return int.TryParse(idClaim, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var productIds = await _db.Favorites
            .Where(f => f.UserId == userId.Value)
            .Select(f => f.ProductId)
            .ToListAsync();

        return Ok(productIds);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> Add(int productId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var exists = await _db.Favorites.AnyAsync(f => f.UserId == userId.Value && f.ProductId == productId);
        if (!exists)
        {
            _db.Favorites.Add(new Favorite { UserId = userId.Value, ProductId = productId });
            await _db.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId.Value && f.ProductId == productId);
        if (favorite != null)
        {
            _db.Favorites.Remove(favorite);
            await _db.SaveChangesAsync();
        }
        return Ok();
    }
}