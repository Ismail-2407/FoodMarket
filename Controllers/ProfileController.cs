using System.Security.Claims;
using FoodMarket.API.DTOs;
using FoodMarket.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.API.Controllers;

[ApiController]
[Route("api/profile")]
//[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db)
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
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        var addresses = await _db.Addresses
            .Where(a => a.UserId == userId.Value)
            .Select(a => new AddressDto { Id = a.Id, FullAddress = a.FullAddress, IsDefault = a.IsDefault })
            .ToListAsync();

        return Ok(new ProfileDto
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            PhotoUrl = user.PhotoUrl,
            Addresses = addresses
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.PhotoUrl = dto.PhotoUrl;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Профиль обновлён" });
    }
    
    [HttpPost("photo")]
    public async Task<IActionResult> UploadPhoto(IFormFile photo)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        if (photo == null || photo.Length == 0)
            return BadRequest("Файл не передан");

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        var uploadsFolder = Path.Combine("wwwroot", "images", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        user.PhotoUrl = $"/images/profiles/{fileName}";
        await _db.SaveChangesAsync();

        return Ok(new { photoUrl = user.PhotoUrl });
    }
}