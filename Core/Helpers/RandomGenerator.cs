namespace Core.Helpers;

public class RandomGenerator
{
    public static string GenerateBookingReference()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("+", "")
            .Replace("/", "")
            .Substring(0, 6)
            .ToUpper();

        return $"BK-{datePart}-{randomPart}";
    }
}