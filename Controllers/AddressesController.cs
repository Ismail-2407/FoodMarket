using System.Security.Claims;
using FoodMarket.API.DTOs;
using FoodMarket.Data;
using FoodMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.API.Controllers;

[ApiController]
[Route("api/addresses")]
//[Authorize]
public class AddressesController : ControllerBase
{
    private readonly AppDbContext _db;

    public AddressesController(AppDbContext db)
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

        var addresses = await _db.Addresses
            .Where(a => a.UserId == userId.Value)
            .Select(a => new AddressDto { Id = a.Id, FullAddress = a.FullAddress, IsDefault = a.IsDefault })
            .ToListAsync();

        return Ok(addresses);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertAddressDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var address = new Address
        {
            UserId = userId.Value,
            FullAddress = dto.FullAddress,
            IsDefault = dto.IsDefault
        };
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        return Ok(new AddressDto { Id = address.Id, FullAddress = address.FullAddress, IsDefault = address.IsDefault });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertAddressDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value);
        if (address == null) return NotFound();

        address.FullAddress = dto.FullAddress;
        address.IsDefault = dto.IsDefault;
        await _db.SaveChangesAsync();

        return Ok(new AddressDto { Id = address.Id, FullAddress = address.FullAddress, IsDefault = address.IsDefault });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized("Пользователь не авторизован");

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId.Value);
        if (address == null) return NotFound();

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Адрес удалён" });
    }
}