using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Encodings;
using MimeKit;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.Drawing;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.RateLimiting;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configurations;
    private readonly IEmailService _emailService;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configurations = configuration;
        _emailService = emailService;
    }

    [Authorize]
    [HttpPost("register")]
    [EnableRateLimiting("fixed")]
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
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Successfully logged out." });
    }

    [HttpPost("login")]
    [EnableRateLimiting("fixed")]
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
            expires: DateTime.Now.AddMonths(1),
            signingCredentials: creds);

        // Write and return the token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("change-password/{userId}")]
    [EnableRateLimiting("fixed")]
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

    [HttpPost("forgot-password")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return BadRequest("Email is required.");
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var pinCode = new Random().Next(100000, 999999).ToString();
        user.PasswordResetCode = pinCode;
        user.PasswordResetCodeExpiration = DateTime.UtcNow.AddMinutes(15);
        await _userManager.UpdateAsync(user);

        var subject = "Password Reset PIN Code";
        var message = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            background-color: #007bff;
            color: #ffffff;
            padding: 10px;
            border-radius: 8px 8px 0 0;
        }}
        .content {{
            padding: 20px;
        }}
        .button {{
            display: inline-block;
            background-color: #28a745;
            color: #ffffff;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 5px;
            font-size: 16px;
            margin-top: 20px;
        }}
        .button:hover {{
            background-color: #218838;
        }}
        .footer {{
            text-align: center;
            font-size: 14px;
            color: #666666;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <p>Hello,</p>
            <p>You have requested to reset your password. Please use the following PIN code to proceed with the reset:</p>
            <h2 style='background-color: #f1f1f1; padding: 10px; border-radius: 5px; text-align: center;'>
                <strong>{pinCode}</strong>
            </h2>
            <p>This PIN code will expire in 15 minutes. If you did not request this change, please ignore this email.</p>
            <p>If you need any assistance, please contact our support team.</p>
        </div>
        <div class='footer'>
            <p>Thank you,<br>Your Company Team</p>
        </div>
    </div>
</body>
</html>";



        await _emailService.SendEmailAsync(user.Email, subject, message);

        return Ok(new { message = "Password reset PIN code has been sent to your email." });
    }


    [HttpPost("reset-password")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) ||
            string.IsNullOrWhiteSpace(model.PinCode) ||
            string.IsNullOrWhiteSpace(model.NewPassword))
        {
            return BadRequest("All fields are required.");
        }

        // Retrieve the user
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Check if PIN code matches and is not expired
        if (user.PasswordResetCode != model.PinCode ||
            user.PasswordResetCodeExpiration < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired PIN code.");
        }

        // Remove the old password
        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
        {
            return BadRequest(removePasswordResult.Errors);
        }

        // Add the new password
        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
        if (addPasswordResult.Succeeded)
        {
            // Clear the PIN code and expiration
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiration = null;
            await _userManager.UpdateAsync(user);

            return Ok("Password has been reset successfully.");
        }

        return BadRequest(addPasswordResult.Errors);
    }




}
