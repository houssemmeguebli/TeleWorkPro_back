using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [Authorize(Policy = "ProjectManagerOnly")]
        [EnableRateLimiting("fixed")]
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
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateProjectManagers(long userId, ProjectManager updateprojectManager)
        {
            var existingProjectManager = await _projectManagerService.GetByIdAsync(userId);
            if (existingProjectManager == null)
            {
                return NotFound("There is no user with this id");
            }
            if (existingProjectManager.role != null)
                existingProjectManager.role = updateprojectManager.role;

            if (existingProjectManager.department != null)
                existingProjectManager.department = updateprojectManager.department;

            if (existingProjectManager.firstName != null)
                existingProjectManager.firstName = updateprojectManager.firstName;

            if (existingProjectManager.lastName != null)
                existingProjectManager.lastName = updateprojectManager.lastName;

            if (existingProjectManager.projectName != null)
                existingProjectManager.projectName = updateprojectManager.projectName;

            if(existingProjectManager.PhoneNumber != null)
                existingProjectManager.PhoneNumber = updateprojectManager.PhoneNumber;

            if (existingProjectManager.Gender != null)
                existingProjectManager.Gender = updateprojectManager.Gender;

            if (existingProjectManager.dateOfbirth != null)
                existingProjectManager.dateOfbirth = updateprojectManager.dateOfbirth;

            if (existingProjectManager.UserStatus != null)
                existingProjectManager.UserStatus = updateprojectManager.UserStatus;


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
        [Authorize(Policy = "ProjectManagerOnly")]
        [EnableRateLimiting("fixed")]
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

        [HttpGet("{managerId}/requests")]
        [Authorize(Policy = "ProjectManagerOnly")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsByEmployeeId(long managerId)
        {
            var requests = await _projectManagerService.GetRequestsByManagerIdAsync(managerId);
            if (requests == null || !requests.Any())
            {
                return NotFound($"No requests found for employee with ID {managerId}");
            }
            return Ok(requests);
        }

    }
}
