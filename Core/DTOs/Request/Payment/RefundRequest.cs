namespace Core.DTOs.Request.Payment;

public class RefundRequest
{
    public string PaymentId { get; set; }
    public long Amount { get; set; }

    /// <summary>
    /// Reasons can only be: duplicate, fraudulent, requested_by_customer
    /// </summary>
    public string Reason { get; set; } = "requested_by_customer";
    public string Currency { get; set; } = "lkr";
}