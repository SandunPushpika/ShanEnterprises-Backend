using Core.DTOs.Request.Other;
using Core.Helpers;
using Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private const string SenderName = "Shan Enterprises";
    
    public async Task SendEmailAsync(MailSettings settings, EmailSendRequest emailSendRequest)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(SenderName, settings.SenderEmail));
        mimeMessage.To.Add(new MailboxAddress(emailSendRequest.ReciepientName, emailSendRequest.To));
        mimeMessage.Subject = emailSendRequest.Subject;
        
        mimeMessage.Body = new TextPart(emailSendRequest.IsBodyHtml ? TextFormat.Html : TextFormat.Plain)
        {
            Text = emailSendRequest.Body
        };

        using var client = new SmtpClient();
        
        await client.ConnectAsync("smtp.gmail.com", settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(settings.SenderEmail, settings.Password);
    
        await client.SendAsync(mimeMessage);
        await client.DisconnectAsync(true);
    }
}