namespace Core.DTOs.Request.User;

public class UserProfileUpdateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? NicPassportNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
}
