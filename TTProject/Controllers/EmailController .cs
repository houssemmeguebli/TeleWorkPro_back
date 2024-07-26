using Microsoft.AspNetCore.Mvc;
using TTProject.Core.Interfaces;

[ApiController]
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
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Message);
        return Ok("Email sent successfully");
    }
}

public class EmailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
}
