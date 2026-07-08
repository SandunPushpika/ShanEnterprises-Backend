using Core.DTOs.Request.Payment;

namespace Infrastructure.Interfaces;

public interface IPaymentService
{
    Task<ExternalPaymentResponse> CreateCheckoutSession(ExternalPaymentRequest request);
}