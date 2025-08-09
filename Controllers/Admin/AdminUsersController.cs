using Microsoft.AspNetCore.Mvc;
using FoodMarket.Services;
using FoodMarket.Models;
using FoodMarket.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodMarket.Controllers.Admin;

[Route("api/admin/users")]
[ApiController]
//[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public AdminUsersController(IUserService userService, AppDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users.Select(u => new
        {
            u.Id,
            u.FullName,
            u.Email
        }));
    }

    [HttpGet("{id}/roles")]
    public async Task<ActionResult<string>> GetUserRoles(int id)
    {
        var (role, found) = await _userService.GetRoleByIdAsync(id);
        if (!found) return NotFound();
        return Ok(role);
    }

    [HttpPost("{id}/add-role")]
    public async Task<IActionResult> AddRole(int id, [FromBody] string role)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
        if (roleEntity == null) return BadRequest("Роль не найдена");

        var alreadyAssigned = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == id && ur.RoleId == roleEntity.Id);
        if (alreadyAssigned) return BadRequest("Роль уже назначена");

        _context.UserRoles.Add(new IdentityUserRole<int>
        {
            UserId = id,
            RoleId = roleEntity.Id
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/remove-role")]
    public async Task<IActionResult> RemoveRole(int id, [FromBody] string role)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
        if (roleEntity == null) return BadRequest("Роль не найдена");

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == id && ur.RoleId == roleEntity.Id);
        if (userRole == null) return BadRequest("Роль у пользователя не найдена");

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
