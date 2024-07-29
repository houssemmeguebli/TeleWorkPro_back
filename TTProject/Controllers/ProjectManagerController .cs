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

    public class ProjectManagerController : ControllerBase
    {
        private readonly IProjectManagerService _projectManagerService;

        public ProjectManagerController(IProjectManagerService userService)
        {
            _projectManagerService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUserById(long userId)
        {
            var projectManagers = await _projectManagerService.GetByIdAsync(userId);
            if (projectManagers == null)
            {
                return NotFound($"There is no user with ID {userId}");
            }
            return Ok(projectManagers);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectManager>>> GetAll()
        {
            var projectManagers = await _projectManagerService.GetAllAsync();
            if (projectManagers == null || !projectManagers.Any())
            {
                return NotFound("There are no users");
            }
            return Ok(projectManagers);
        }


        [HttpPost]
        public async Task<ActionResult<ProjectManager>> CreateUser(ProjectManager projectManager)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _projectManagerService.AddAsync(projectManager);
                return CreatedAtAction(nameof(GetUserById), new { userId = projectManager.Id }, projectManager);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding user");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProjectManagers(long userId, ProjectManager updateprojectManager)
        {
            var existingProjectManager = await _projectManagerService.GetByIdAsync(userId);
            if (existingProjectManager == null)
            {
                return NotFound("There is no user with this id");
            }

            existingProjectManager.PhoneNumber = updateprojectManager.PhoneNumber;
            existingProjectManager.role = updateprojectManager.role;
            existingProjectManager.department = updateprojectManager.department;
            existingProjectManager.firstName = updateprojectManager.firstName;
            existingProjectManager.lastName = updateprojectManager.lastName;
            existingProjectManager.Gender = updateprojectManager.Gender;
            existingProjectManager.dateOfbirth = updateprojectManager.dateOfbirth;

            await _projectManagerService.UpdateAsync(existingProjectManager);

            return NoContent();

        }
        [HttpGet("getByUserId/{userID}")]
        public async Task<IActionResult> GetUserByName(long userID)
        {
            try
            {
                var projectManagers = await _projectManagerService.GetUserByNameAsync(userID);


                return Ok(new { UserId = userID, FirstName = projectManagers.firstName, LastName = projectManagers.lastName });
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteProjectManagers(long userId)
        {
            var projectManagers = await _projectManagerService.GetByIdAsync(userId);
            if (projectManagers == null)
            {
                return NotFound();
            }

            await _projectManagerService.DeleteAsync(projectManagers);
            return NoContent();
        }
    }
}
