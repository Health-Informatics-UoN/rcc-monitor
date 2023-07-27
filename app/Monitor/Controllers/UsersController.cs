using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Monitor.Auth;
using Monitor.Config;
using Monitor.Data.Entities.Identity;
using Monitor.Models.Account;
using Monitor.Models.Emails;
using Monitor.Models.User;
using Monitor.Services;
using Monitor.Services.EmailServices;

namespace Monitor.Controllers;

[ApiController]
[Authorize(nameof(AuthPolicies.CanManageUsers))]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _users;
  private readonly RoleManager<IdentityRole> _roles;
  private readonly UserAccountOptions _userAccountOptions;
  private readonly TokenIssuingService _tokens;
  private readonly AccountEmailService _accountEmail;
  private readonly UserService _user;

  public UsersController(
    UserManager<ApplicationUser> users,
    RoleManager<IdentityRole> roles,
    IOptions<UserAccountOptions> userAccountOptions,
    TokenIssuingService tokens,
    AccountEmailService accountEmail,
    UserService user)
  {
    _users = users;
    _roles = roles;
    _userAccountOptions = userAccountOptions.Value;
    _tokens = tokens;
    _accountEmail = accountEmail;
    _user = user;
  }

  /// <summary>
  /// Get users list
  /// </summary>
  /// <returns>users list with their associated roles</returns>
  [HttpGet]
  public async Task<List<UserModel>> List()
  {
    var list = await _users.Users.ToListAsync();

    var usersList = new List<UserModel>();
    foreach (var x in list)
    {
      var roles = await _users.GetRolesAsync(x); // Get user roles
      var user = new UserModel
      {
        Id = x.Id,
        FullName = x.FullName,
        Email = x.Email,
        EmailConfirmed = x.EmailConfirmed,
        Roles = new List<string>(roles), // Assign list of roles
      }; ;
      
      usersList.Add(user);
    }
    return usersList; // return users list
  }

  /// <summary>
  /// Get user
  /// </summary>
  /// <param name="id">user id</param>
  /// <returns>user matching the id</returns>
  public async Task<UserModel> Get(string id)
  {
    var userFound = await _users.FindByIdAsync(id);
    var roles = await _users.GetRolesAsync(userFound); // Get user roles
    var user = new UserModel
    {
      Id = userFound.Id,
      FullName = userFound.FullName,
      Email = userFound.Email,
      EmailConfirmed = userFound.EmailConfirmed,
      Roles = new List<string>(roles) // Assign list of roles
    };
    return user; // return user
  }

  /// <summary>
  /// Update user roles
  /// </summary>
  /// <param name="userModel"></param>
  /// <param name="id"></param>
  [HttpPut("userRoles/{id}")]
  public async Task<IActionResult> SetUserRoles(string id, [FromBody] UserModel userModel)
  {
    // Check minimum roles is selected
    if (userModel.Roles.Count < 1)
      throw new ArgumentException("Minimum of one role required");

    // check role is available and valid
    var rolesAvailable = await _roles.Roles.ToListAsync(); // get list of available roles
    var valid = userModel.Roles.All(x => rolesAvailable.Any(y => x == y.NormalizedName));
    if (!valid) throw new ArgumentException("Invalid roles");

    var user = await _users.FindByIdAsync(id); // Find the user
    if (user is null) return NotFound(); // return 404 if user not found

    var currentRoles = await _users.GetRolesAsync(user); // Get user's current roles
    await _users.RemoveFromRolesAsync(user, currentRoles); // remove the existing roles
    await _users.AddToRolesAsync(user, userModel.Roles); // assign the new roles to the user
    return NoContent();
  }

  /// <summary>
  /// Start email change process. Generate email change link and send it to the user
  /// </summary>
  /// <param name="userModel"></param>
  /// <param name="id"></param>
  [HttpPut("userEmail/{id}")]
  public async Task<IActionResult> ChangeEmail(string id, UserModel userModel)
  {
    var user = await _users.FindByIdAsync(id); // Find the user
    if (user is null) return NotFound(); // return 404 if user not found

    // check if email already exist
    var isEmailExist = await _users.FindByEmailAsync(userModel.Email);
    if (isEmailExist is not null) return BadRequest(new SetEmailResult() { IsExistingEmail = true });

    // check if email is confirmed. Only allow email change if the user has their current email confirmed
    if (!user.EmailConfirmed) return BadRequest(new SetEmailResult() { IsUnconfirmedAccount = true });

    // check if new email is valid i.e. check if this new email would satisfy the registration rule
    if (!await _user.CanRegister(userModel.Email)) return BadRequest(new SetEmailResult() { IsNotAllowlisted = true });

    if (_userAccountOptions.SendEmail) // Check the config and send email Change link email if true
      await _tokens.SendEmailChange(user, userModel.Email);

    if (_userAccountOptions.GenerateLink) // return email change link if true
      return Ok(await _tokens.GenerateEmailChangeLink(user, userModel.Email)); // Check 200 status in the client for EmailChangeLink

    return NoContent(); // Return 204 if no EmailChangeLink 
  }

  /// <summary>
  /// Delete User by ID
  /// </summary>
  /// <param name="id"></param>
  /// <param name="userModel"></param>
  /// <exception cref="KeyNotFoundException"></exception>
  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id, [FromBody] UserModel userModel)
  {
    var user = await _users.FindByIdAsync(id);
    if (user is null) return NotFound();
    await _users.DeleteAsync(user);
    if (userModel.SendUpdateEmail) // Check if send update email is true
      await _accountEmail.SendDeleteUpdate(
        new EmailAddress(userModel.Email) { Name = user.FullName });
    return NoContent();
  }
}
