namespace Core.DTOs.Request.Payment;

public class ExternalPaymentRequest
{
    public string Vehicle { get; set; }
    public long? Amount { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string BookingReference { get; set; }
    public string Currency { get; set; } = "lkr";
}