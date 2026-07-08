using Core.DTOs.Request.Payment;
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
    private readonly AppSettings _appSettings;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<AppSettings> appSettings, ILogger<StripeService> logger)
    {
        _appSettings = appSettings.Value;
        _stripeClient = new StripeClient(_appSettings.StripeConfigs.SecretKey);
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
                            Description = $"{{request.From:dd MMM yyyy}} - {{request.To:dd MMM yyyy}}"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            Currency = request.Currency,
            SuccessUrl = "https://example.com/success",
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
            throw new Exception("Payment could not be created.");
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