namespace Core.DTOs.Request.Payment;

public class ExternalPaymentResponse
{
    public string ExternalPaymentId { get; set; }
    public string PaymentUrl { get; set; }
}