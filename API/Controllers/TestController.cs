using Core.DTOs.Request.Other;
using Core.DTOs.Request.Payment;
using Core.Helpers;
using Core.Helpers.Templates;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController(IEmailService emailService, IOptions<AppSettings> appSettings, IPaymentService paymentService) : Controller
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

    [HttpGet("test-stripe")]
    public async Task<ActionResult> GetStripeCheckout()
    {
        var session = await paymentService.CreateCheckoutSession(new ExternalPaymentRequest()
        {
            Amount = 3000,
            To = DateTime.UtcNow.AddDays(4),
            From = DateTime.UtcNow.AddDays(2),
            Vehicle = "KL-2344",
            BookingReference = "BLK-223355"
        });

        return Ok(session);
    }
    
    [HttpGet("test-stripe-paid")]
    public async Task<ActionResult> GetStripeCheckoutPaid(string paymentOrSessionId)
    {
        var session = await paymentService.IsPaid(paymentOrSessionId);

        return Ok(new
        {
            session.Item1,
            session.Item2,
        });
    }
    
    [HttpGet("test-stripe-refund")]
    public async Task<ActionResult> GetStripeCheckoutRefund(string paymentOrSessionId)
    {
        await paymentService.MakeRefund(new RefundRequest()
        {
            Amount = 3000,
            PaymentId = paymentOrSessionId
        });

        return Ok();
    }
}