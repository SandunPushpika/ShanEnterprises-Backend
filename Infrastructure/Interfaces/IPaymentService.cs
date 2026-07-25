using Core.DTOs.Request.Payment;
using Core.Enums;

namespace Infrastructure.Interfaces;

public interface IPaymentService
{
    Task<ExternalPaymentResponse> CreateCheckoutSession(ExternalPaymentRequest request);
    Task<(string, PaymentStatus)> IsPaid(string paymentOrSessionId);
    Task MakeRefund(RefundRequest refundRequest);
}