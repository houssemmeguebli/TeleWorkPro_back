using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TTProject.Core.Entities;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configurations;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configurations = configuration;
    }

    [Authorize]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        User user;

        if (model.role == Role.ProjectManager)
        {
            user = new ProjectManager
            {
                UserName = model.Email,
                Email = model.Email,
                firstName = model.FirstName,
                lastName = model.LastName,
                PhoneNumber = model.phone,
                department = model.department,
                projectName = model.projectName,
                role = model.role,
                Gender = model.Gender,
                dateOfbirth = model.dateOfbirth,
                UserStatus = UserStatus.Inactive // Requires password change on first login
            };
        }
        else if (model.role == Role.Employee)
        {
            user = new Employee
            {
                UserName = model.Email,
                Email = model.Email,
                firstName = model.FirstName,
                lastName = model.LastName,
                PhoneNumber = model.phone,
                department = model.department,
                position = model.position,
                role = model.role,
                Gender = model.Gender,
                dateOfbirth = model.dateOfbirth,
                UserStatus = UserStatus.Inactive // Requires password change on first login
            };
        }
        else
        {
            return BadRequest("Invalid role specified.");
        }

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.role.ToString());
            return Ok();
        }

        return BadRequest(result.Errors);
    }
   
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Successfully logged out." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { Token = GenerateJwtToken(user, roles) });
        }

        return Unauthorized(new { message = "Invalid login attempt." });
    }
    

    private string GenerateJwtToken(User user, IList<string> roles)
    {
        // Create a list of claims
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("id", user.Id.ToString()),  // Include the user's id
        new Claim("role", user.role.ToString()),
        new Claim("firstName", user.firstName ?? ""),

        new Claim("lastName", user.lastName ?? ""),
        new Claim("department", user.department ?? "")
    };

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Create a symmetric security key and credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurations["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token
        var token = new JwtSecurityToken(
            issuer: _configurations["Jwt:Issuer"],
            audience: _configurations["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: creds);

        // Write and return the token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("change-password/{userId}")]
    public async Task<IActionResult> ChangePassword(long userId, [FromBody] ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return NotFound("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
            return Ok("Password changed successfully.");

        return BadRequest(result.Errors);
    }
   
    [HttpGet("user-id")]
    public async Task<IActionResult> GetUserIdByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        // Assuming UserStatus is a property on your user model
        var userStatus = user.UserStatus;

        return Ok(new
        {
            UserId = user.Id,
            Role = role,
            UserStatus = userStatus  
        });
    }


}
