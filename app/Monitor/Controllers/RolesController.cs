using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitor.Auth;
using Monitor.Models;

namespace Monitor.Controllers;

[ApiController]
[Authorize(nameof(AuthPolicies.CanManageUsers))]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
  private readonly RoleManager<IdentityRole> _role;

  public RolesController(RoleManager<IdentityRole> role)
  {
    _role = role;
  }

  [HttpGet]
  public async Task<ActionResult<List<Role>>> List()
  {
    var rolesList = await _role.Roles.ToListAsync();
    return rolesList.ConvertAll<Models.Role>(x => new Role{Id = x.Id, Name = x.NormalizedName});
  }

  [HttpGet("{roleId}")]
  public async Task<ActionResult<Role>> Get(string roleId)
  {
    var role = await _role.FindByIdAsync(roleId);
    if (role is null) throw new KeyNotFoundException();
    return new Role { Id = role.Id, Name = role.NormalizedName };
  }
}
