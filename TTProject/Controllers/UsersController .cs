using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TTProject.Application.Services;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;
using TTProject.Infrastructure.Repositories;

namespace TTProject.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUserById(long userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"There is no user with ID {userId}");
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            if (users == null || !users.Any())
            {
                return NotFound("There are no users");
            }
            return Ok(users);
        }


        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _userService.AddAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { userId = user.userId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding user");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(long userId, User updatedUser)
        {
            var existingUser = await _userService.GetByIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound("There is no user with this id");
            }

             existingUser.phone= updatedUser.phone;
            existingUser.email= updatedUser.email;
            existingUser.password= updatedUser.password;
             existingUser.role= updatedUser.role;
            existingUser.department= updatedUser.department;
            existingUser.firstName= updatedUser.firstName;
            existingUser.lastName= updatedUser.lastName;

            await _userService.UpdateAsync(existingUser);

            return NoContent();

        }
        [HttpGet("getByUserId/{userID}")]
        public async Task<IActionResult> GetUserByName(long userID)
        {
            try
            {
                var user = await _userService.GetUserByNameAsync(userID);

               
                return Ok(new { UserId = userID, FirstName = user.firstName, LastName = user.lastName });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteAsync(user);
            return NoContent();
        }
    }
}
