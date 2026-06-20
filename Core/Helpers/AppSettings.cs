namespace Core.Helpers;

public class AppSettings
{
    public string DefaultConnection { get; set; }
    public string AdminKeyCode { get; set; }
    public string BlobConnectionString { get; set; }
    public JwtSettings JwtSettings { get; set; }
    public MailSettings MailSettings { get; set; }
    public GoogleSettings Google { get; set; }
}

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecurityKey { get; set; }
}

public class MailSettings
{
    public string SenderEmail { get; set; }
    public string Server { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
}

public class GoogleSettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scopes { get; set; }
    public string CallbackUrl { get; set; }
}