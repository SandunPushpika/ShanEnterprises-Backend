using Core.DTOs.Request.Other;
using Core.Helpers;
using Core.Helpers.Templates;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController(IEmailService emailService, IOptions<AppSettings> appSettings) : Controller
{
    [HttpGet]
    public async Task<IActionResult> SendEmail(string email, string subject)
    {
        var mailSettigns = appSettings.Value.MailSettings;
        var emailRequest = new EmailSendRequest()
        {
            Body = VerificationEmailTemplate.Generate("Testing Customer", "112233"),
            Subject = subject,
            IsBodyHtml = true,
            To = email,
            ReciepientName = "Sandun"
        };
        
        await emailService.SendEmailAsync(mailSettigns, emailRequest);
        return Ok();
    }
}