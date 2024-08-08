using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTProject.Application.Services;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;

namespace TTProject.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("{employeeId}")]
    
        public async Task<ActionResult<User>> GetUserById(long employeeId)
        {
            var employee = await _employeeService.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"There is no employee with ID {employeeId}");
            }
            return Ok(employee);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _employeeService.AddAsync(employee);
                return CreatedAtAction(nameof(GetUserById), new { employeeId = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding employee");
            }
        }

        [HttpGet]
    
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            if (employees == null || !employees.Any())
            {
                return NotFound("There are no employees");
            }
            return Ok(employees);
        }

       [HttpPut("{employeeId}")]
public async Task<IActionResult> UpdateUser(long employeeId, Employee updatedEmployee)
{
    var existingEmployee = await _employeeService.GetByIdAsync(employeeId);
    if (existingEmployee == null)
    {
        return NotFound("There is no employee with this ID");
    }

    // Only update fields that are provided in updatedEmployee
    if (updatedEmployee.PhoneNumber != null)
        existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
     
    if (updatedEmployee.role != null)
        existingEmployee.role = updatedEmployee.role;

    if (updatedEmployee.department != null)
        existingEmployee.department = updatedEmployee.department;

    if (updatedEmployee.firstName != null)
        existingEmployee.firstName = updatedEmployee.firstName;

    if (updatedEmployee.lastName != null)
        existingEmployee.lastName = updatedEmployee.lastName;

    if (updatedEmployee.position != null)
        existingEmployee.position = updatedEmployee.position;

    if (updatedEmployee.Gender != null)
        existingEmployee.Gender = updatedEmployee.Gender;
    if(updatedEmployee.PhoneNumber!= null)
                existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;

    if (updatedEmployee.dateOfbirth != null)
        existingEmployee.dateOfbirth = updatedEmployee.dateOfbirth;

    if (updatedEmployee.UserStatus != null)
        existingEmployee.UserStatus = updatedEmployee.UserStatus;

    await _employeeService.UpdateAsync(existingEmployee);

    return NoContent();
}

        [HttpGet("{employeeId}/requests")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsByEmployeeId(long employeeId)
        {
            var requests = await _employeeService.GetRequestsByEmployeeIdAsync(employeeId);
            if (requests == null || !requests.Any())
            {
                return NotFound($"No requests found for employee with ID {employeeId}");
            }
            return Ok(requests);
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteEmployee(long userId)
        {
            var employees = await _employeeService.GetByIdAsync(userId);
            if (employees == null)
            {
                return NotFound();
            }

            await _employeeService.DeleteAsync(employees);
            return NoContent();
        }
    }
}

