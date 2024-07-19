using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTProject.Application.Services;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;

namespace TTProject.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        private readonly IUserService _userService;

        public RequestController(IRequestService requestService, IUserService userService)
        {
            _requestService = requestService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TTRequest>>> GetAll()
        {
            var requests = await _requestService.GetAllAsync();
            if (requests == null || !requests.Any())
            {
                return NotFound("There are no requests");
            }
            return Ok(requests);
        }
        /*
        [HttpPost]
        public async Task<ActionResult<TTRequest>> CreateRequest(TTRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _requestService.AddAsync(request);
            return CreatedAtAction(nameof(GetRequestById), new { requestId = request.RequestId }, request);
        }
        */
        [HttpPost]
        public async Task<ActionResult<TTRequest>> CreateRequest(TTRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

  
            var staticRole = Role.Employee;  // Replace with Role.ProjectManager or Role.Employee for different scenarios

            if (staticRole == Role.ProjectManager)
            {
                request.status = Status.Approved;
            }
            else if (staticRole == Role.Employee)
            {
                request.status = Status.Pending;
            }

         
             await _requestService.AddAsync(request);

            // Simulate returning CreatedAtAction
            return CreatedAtAction(nameof(GetRequestById), new { requestId = request.RequestId }, request);
        }


        [HttpGet("{requestId}")]
        public async Task<ActionResult<TTRequest>> GetRequestById(long requestId)
        {
            var request = await _requestService.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound("There is no request with this id");
            }
            return Ok(request);
        }
        [HttpDelete("{requestId}")]
        public async Task<IActionResult> DeleteRequest(long requestId)
        {
            var request = await _requestService.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound("There is no request with this id");
            }

            await _requestService.DeleteAsync(request);
            return NoContent();
        }
        [HttpPut("{requestId}")]
        public async Task<IActionResult> UpdateRequest(long requestId, TTRequest updatedRequest)
        {
            var existingRequest = await _requestService.GetByIdAsync(requestId);
            if (existingRequest == null)
            {
                return NotFound("There is no request with this id");
            }

            existingRequest.status = updatedRequest.status;
            existingRequest.startDate = updatedRequest.startDate;
            existingRequest.endDate = updatedRequest.endDate;
            existingRequest.comment = updatedRequest.comment;
            existingRequest.note = updatedRequest.note;


            await _requestService.UpdateAsync(existingRequest);

            return NoContent();
        }
        /*
        [HttpPost("requests/{requestId}/status")]
        public async Task<ActionResult> UpdateRequestStatus(long requestId, [FromBody] string status)
        {
            var request = await _requestService.GetByIdAsync(requestId);

            if (request == null)
            {
                return NotFound("Request not found");
            }

            switch (status.ToLower())
            {
                case "approve":
                    request.status = Status.Approved;
                    break;
                case "reject":
                    request.status = Status.Rejected;
                    break;
                default:
                    return BadRequest("Invalid status action. Allowed values are 'approve' or 'reject'.");
            }

            await _requestService.UpdateAsync(request);

            return Ok();
        }
        */
        [HttpPost("requests/{requestId}/status")]
        public async Task<ActionResult> UpdateRequestStatus(long requestId, [FromBody] TTRequest request)
        {
            var existingRequest = await _requestService.GetByIdAsync(requestId);

            if (existingRequest == null)
            {
                return NotFound("Request not found");
            }

            switch (request.status)
            {
                case Status.Approved:
                    existingRequest.status = Status.Approved;
                    // Optionally update start date and/or end date
                    existingRequest.startDate = DateTime.UtcNow.Date; // Replace with your logic
                    existingRequest.endDate = DateTime.UtcNow.Date.AddDays(7); // Replace with your logic
                    break;
                case Status.Rejected:
                    existingRequest.status = Status.Rejected;
                    break;
                case Status.Updated:
                    // Handle "updated" status where manager can modify start date, end date, or both
                    if (request.startDate != default(DateTime))
                    {
                        existingRequest.startDate = request.startDate;
                    }
                    if (request.endDate != default(DateTime))
                    {
                        existingRequest.endDate = request.endDate;
                    }
                    existingRequest.status = Status.Updated;
                    break;
                default:
                    return BadRequest("Invalid status action. Allowed values are 'Approved', 'Rejected', or 'Updated'.");
            }

            await _requestService.UpdateAsync(existingRequest);

            return Ok();
        }




    }
}
