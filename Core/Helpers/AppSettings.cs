namespace Core.Helpers;

public class AppSettings
{
    public string DefaultConnection { get; set; }
    public JwtSettings JwtSettings { get; set; }
}

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecurityKey { get; set; }
}