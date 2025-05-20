using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FoodMarket.Models;

namespace FoodMarket.Controllers.Admin;

[Route("api/admin/users")]
[ApiController]
//[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AdminUsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAllUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.FullName,
            u.Email
        }).ToList();

        return Ok(users);
    }

    [HttpGet("{id}/roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [HttpPost("{id}/add-role")]
    public async Task<IActionResult> AddRole(string id, [FromBody] string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost("{id}/remove-role")]
    public async Task<IActionResult> RemoveRole(string id, [FromBody] string role)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }
}