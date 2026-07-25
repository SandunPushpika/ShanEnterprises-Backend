using Core.Enums;

namespace Core.Entities;

public class Payments
{
    public long Id { get; set; }
    public long BookingId { get; set; }
    public long CustomerId { get; set; }
    public string? TransactionReference { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentGateway { get; set; } = "stripe";
    public DateTime? PaidAt { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
}