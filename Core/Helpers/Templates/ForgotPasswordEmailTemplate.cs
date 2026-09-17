namespace Core.Helpers.Templates;

using System;

public static class ForgotPasswordEmailTemplate
{
    public static string Generate(string customerName, string verificationCode)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>

<body style='margin:0; padding:0; background-color:#f4f8ff; font-family:Arial, Helvetica, sans-serif;'>

    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' style='background-color:#f4f8ff; padding:40px 0;'>
        <tr>
            <td align='center'>

                <!-- Container -->
                <table role='presentation' width='600' cellspacing='0' cellpadding='0' 
                       style='background:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

                    <!-- Header -->
                    <tr>
                        <td style='background:linear-gradient(135deg,#1e66ff,#4f8cff); padding:25px; text-align:center;'>
                            <h1 style='margin:0; color:#ffffff; font-size:22px;'>
                                Reset Your Password
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
                                We received a request to reset the password for your DriveLux account. Please use the verification code below to reset your password.
                            </p>

                            <!-- Code Box -->
                            <div style='text-align:center; margin:30px 0;'>
                                <div style='display:inline-block; padding:15px 25px; font-size:28px; letter-spacing:6px;
                                            font-weight:bold; color:#1e66ff; background:#f0f6ff;
                                            border:2px dashed #1e66ff; border-radius:8px;'>
                                    {verificationCode}
                                </div>
                            </div>

                            <p style='font-size:14px; color:#666666; line-height:1.6;'>
                                This code will expire in <strong>10 minutes</strong>.
                            </p>

                            <div style='margin:20px 0; padding:12px 16px; background-color:#fff8e6; border-left:4px solid #f59e0b; border-radius:4px;'>
                                <p style='font-size:13px; color:#92400e; margin:0; line-height:1.5;'>
                                    <strong>Security notice:</strong> If you did not request a password reset, please ignore this email. Your account remains secure and no changes have been made.
                                </p>
                            </div>

                            <hr style='border:none; border-top:1px solid #eeeeee; margin:25px 0;' />

                            <p style='font-size:13px; color:#999999; text-align:center; margin:0;'>
                                © {DateTime.UtcNow.Year} DriveLux. All rights reserved.
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
