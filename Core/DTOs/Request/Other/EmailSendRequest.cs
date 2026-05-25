namespace Core.DTOs.Request.Other;

public class EmailSendRequest
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string? ReciepientName { get; set; }
    public bool IsBodyHtml { get; set; }
}