using Core.DTOs.Request.Other;
using Core.Helpers;

namespace Infrastructure.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(MailSettings settings, EmailSendRequest emailSendRequest);
}