using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TTProject.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace TTProject.Presentation.Controllers
{
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
        
  
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model.role == Role.ProjectManager)
            {
                var user = new ProjectManager
                {
                    UserName = model.Email,
                    Email = model.Email,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    PhoneNumber = model.phone,
                    department = model.department,
                    projectName = model.projectName,
                    role = model.role,

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign Role
                    await _userManager.AddToRoleAsync(user, "ProjectManager");
                    return Ok();
                }

                return BadRequest(result.Errors);
            }
            else if (model.role == Role.Employee)
            {
                var user = new Employee
                {
                    UserName = model.Email,
                    Email = model.Email,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    PhoneNumber = model.phone,
                    department = model.department,
                    position = model.position
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign Role
                    await _userManager.AddToRoleAsync(user, "Employee");
                    return Ok();
                }

                return BadRequest(result.Errors);
            }
            else
            {
                return BadRequest("Invalid role specified.");
            }
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
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Successfully logged out." });
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
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // Write and return the token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}
