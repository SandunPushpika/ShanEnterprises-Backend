namespace Core.Helpers.Templates;

public static class BookingEmailTemplates
{
    public static string GenerateBookingConfirmed(
        string customerName,
        string bookingReferenceId,
        string vehicleNumber,
        string vehicleModel,
        string branchName,
        string pickupDate,
        string returnDate)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Booking Confirmed</title>
</head>

<body style='margin:0; padding:0; background-color:#f4f8ff; font-family:Arial, Helvetica, sans-serif;'>

<table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color:#f4f8ff; padding:40px 0;'>
<tr>
<td align='center'>

<table role='presentation' width='600' cellspacing='0' cellpadding='0'
       style='background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

    <!-- Header -->
    <tr>
        <td style='background:linear-gradient(135deg,#16a34a,#22c55e); padding:25px; text-align:center;'>
            <h1 style='margin:0; color:#ffffff; font-size:22px;'>
                Booking Confirmed
            </h1>
        </td>
    </tr>

    <!-- Body -->
    <tr>
        <td style='padding:30px; color:#333333;'>

            <p style='font-size:16px; margin:0 0 15px 0;'>
                Hello <strong>{customerName}</strong>,
            </p>

            <p style='font-size:15px; line-height:1.6; margin:0 0 20px 0;'>
                Great news! Your vehicle booking has been successfully confirmed.
            </p>

            <!-- Booking Reference -->
            <div style='text-align:center; margin:30px 0;'>
                <div style='display:inline-block; padding:15px 25px; font-size:22px;
                            font-weight:bold; color:#16a34a; background:#f0fdf4;
                            border:2px dashed #16a34a; border-radius:8px;'>
                    Ref: {bookingReferenceId}
                </div>
            </div>

            <!-- Details -->
            <table width='100%' cellpadding='8' cellspacing='0'
                   style='border-collapse:collapse; background:#fafafa; border-radius:8px;'>

                <tr>
                    <td style='font-weight:bold;'>Vehicle</td>
                    <td>{vehicleModel}</td>
                </tr>

                <tr>
                    <td style='font-weight:bold;'>Vehicle Number</td>
                    <td>{vehicleNumber}</td>
                </tr>

                <tr>
                    <td style='font-weight:bold;'>Branch</td>
                    <td>{branchName}</td>
                </tr>

                <tr>
                    <td style='font-weight:bold;'>Pickup Date</td>
                    <td>{pickupDate}</td>
                </tr>

                <tr>
                    <td style='font-weight:bold;'>Return Date</td>
                    <td>{returnDate}</td>
                </tr>

            </table>

            <p style='font-size:14px; color:#666666; line-height:1.6; margin-top:25px;'>
                Please keep your booking reference ID for future inquiries and vehicle collection.
            </p>

            <hr style='border:none; border-top:1px solid #eeeeee; margin:25px 0;' />

            <p style='font-size:13px; color:#999999; text-align:center; margin:0;'>
                © {DateTime.UtcNow.Year} Your Company. All rights reserved.
            </p>

        </td>
    </tr>

</table>

</td>
</tr>
</table>

</body>
</html>";
    }

    public static string GenerateBookingPaymentFailed(
        string customerName,
        string bookingReferenceId,
        string vehicleNumber)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Booking Payment Failed</title>
</head>

<body style='margin:0; padding:0; background-color:#f4f8ff; font-family:Arial, Helvetica, sans-serif;'>

<table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color:#f4f8ff; padding:40px 0;'>
<tr>
<td align='center'>

<table role='presentation' width='600' cellspacing='0' cellpadding='0'
       style='background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

    <!-- Header -->
    <tr>
        <td style='background:linear-gradient(135deg,#dc2626,#ef4444); padding:25px; text-align:center;'>
            <h1 style='margin:0; color:#ffffff; font-size:22px;'>
                Booking Rejected
            </h1>
        </td>
    </tr>

    <!-- Body -->
    <tr>
        <td style='padding:30px; color:#333333;'>

            <p style='font-size:16px; margin:0 0 15px 0;'>
                Hello <strong>{customerName}</strong>,
            </p>

            <p style='font-size:15px; line-height:1.6; margin:0 0 20px 0;'>
                Unfortunately, your booking could not be completed because the payment was unsuccessful.
            </p>

            <div style='background:#fef2f2; border-left:4px solid #dc2626;
                        padding:15px; border-radius:6px; margin-bottom:25px;'>
                <strong>Payment Failure Detected</strong><br/>
                Your booking has been automatically cancelled.
            </div>

            <table width='100%' cellpadding='8' cellspacing='0'
                   style='border-collapse:collapse; background:#fafafa; border-radius:8px;'>

                <tr>
                    <td style='font-weight:bold;'>Booking Reference</td>
                    <td>{bookingReferenceId}</td>
                </tr>

                <tr>
                    <td style='font-weight:bold;'>Vehicle Number</td>
                    <td>{vehicleNumber}</td>
                </tr>

            </table>

            <p style='font-size:14px; color:#666666; line-height:1.6; margin-top:25px;'>
                Please try making the booking again or contact our support team if the issue persists.
            </p>

            <hr style='border:none; border-top:1px solid #eeeeee; margin:25px 0;' />

            <p style='font-size:13px; color:#999999; text-align:center; margin:0;'>
                © {DateTime.UtcNow.Year} Your Company. All rights reserved.
            </p>

        </td>
    </tr>

</table>

</td>
</tr>
</table>

</body>
</html>";
    }
}