using Core.Enums;

namespace Core.Helpers;

public static class EnumMapper
{
    public static PaymentStatus MapStripePaymentStatus(string paymentStatus)
    {
        switch (paymentStatus)
        {
            case "paid":
                return PaymentStatus.COMPLETED;
            case "unpaid":
            default:
                return PaymentStatus.PENDING;
        }
    }
}