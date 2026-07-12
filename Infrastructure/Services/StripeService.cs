using Core.DTOs.Request.Payment;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services;

public class StripeService : IPaymentService
{
    private readonly StripeClient _stripeClient;
    private readonly StripeConfig _stripeConfig;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<AppSettings> appSettings, ILogger<StripeService> logger)
    {
        _stripeConfig = appSettings.Value.StripeConfigs;
        _stripeClient = new StripeClient(_stripeConfig.SecretKey);
        _logger = logger;
    }
    
    public async Task<ExternalPaymentResponse> CreateCheckoutSession(ExternalPaymentRequest request)
    {
        ValidateRequest(request);
        
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = request.Amount * 100,
                        Currency = request.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = request.Vehicle,
                            Description = $"{request.From.ToLongDateString()} - {request.To.ToLongDateString()}"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            Currency = request.Currency,
            SuccessUrl = $"{_stripeConfig.RedirectUri}?session_id={{CHECKOUT_SESSION_ID}}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            ClientReferenceId = request.BookingReference
        };

        try
        {
            var session = await _stripeClient.V1.Checkout.Sessions.CreateAsync(options);
            return new ExternalPaymentResponse()
            {
                PaymentUrl = session.Url,
                ExternalPaymentId = session.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new PaymentException("Payment could not be created.");
        }
    }

    public async Task<(string ,PaymentStatus)> IsPaid(string paymentOrSessionId)
    {
        try
        {
            var session = await _stripeClient.V1.Checkout.Sessions.GetAsync(paymentOrSessionId);
            return (session.PaymentIntentId, EnumMapper.MapStripePaymentStatus(session.PaymentStatus));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new PaymentException("Could not fetch payment details");
        }
    }

    public async Task MakeRefund(RefundRequest refundRequest)
    {
        try
        {
            var refundCreateOptions = new RefundCreateOptions()
            {
                Amount = refundRequest.Amount * 100,
                Reason = refundRequest.Reason,
                PaymentIntent = refundRequest.PaymentId,
            };
            await _stripeClient.V1.Refunds.CreateAsync(refundCreateOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new PaymentException("Could not make refund");
        }
    }

    private void ValidateRequest(ExternalPaymentRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new ArgumentException("Currency is required.");

        if (request.From >= request.To)
            throw new ArgumentException("Invalid booking period.");

        if (string.IsNullOrWhiteSpace(request.Vehicle))
            throw new ArgumentException("Vehicle is required.");
    }
}