using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TTProject.Core.Interfaces;

[ApiController]
[Authorize]
[Route("api/[controller]")]

public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    [Route("send")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Message);
        return Ok("Email sent successfully");
    }
   
    [HttpPost("sendList")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequestList emailRequest)
    {
        if (emailRequest.Emails == null || emailRequest.Emails.Count == 0)
        {
            return BadRequest("Email list is empty.");
        }

        try
        {
            await _emailService.SendEmailsAsyncList(emailRequest.Emails, emailRequest.Subject, emailRequest.Body);
            return Ok("Emails sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

public class EmailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}
public class EmailRequestList
{
    public List<string> Emails { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}