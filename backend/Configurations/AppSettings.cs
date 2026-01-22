using System.Text.Json.Serialization;

namespace BadNews.Configurations;

public class AppSettings
{
    [JsonPropertyName("Database")]
    public DatabaseSettings Database { get; set; } = new();

    [JsonPropertyName("Twilio")]
    public TwilioSettings Twilio { get; set; } = new();

    [JsonPropertyName("MercadoPago")]
    public MercadoPagoSettings MercadoPago { get; set; } = new();

    [JsonPropertyName("SendGrid")]
    public SendGridSettings SendGrid { get; set; } = new();

    [JsonPropertyName("Jwt")]
    public JwtSettings Jwt { get; set; } = new();
}

public class DatabaseSettings
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; } = "";
}

public class TwilioSettings
{
    [JsonPropertyName("AccountSid")]
    public string AccountSid { get; set; } = "";

    [JsonPropertyName("AuthToken")]
    public string AuthToken { get; set; } = "";

    [JsonPropertyName("PhoneNumber")]
    public string PhoneNumber { get; set; } = "";
}

public class MercadoPagoSettings
{
    [JsonPropertyName("AccessToken")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("PublicKey")]
    public string PublicKey { get; set; } = "";
}

public class SendGridSettings
{
    [JsonPropertyName("ApiKey")]
    public string ApiKey { get; set; } = "";

    [JsonPropertyName("FromEmail")]
    public string FromEmail { get; set; } = "";
}

public class JwtSettings
{
    [JsonPropertyName("Secret")]
    public string Secret { get; set; } = "";

    [JsonPropertyName("ExpirationMinutes")]
    public int ExpirationMinutes { get; set; } = 60;

    [JsonPropertyName("Issuer")]
    public string Issuer { get; set; } = "";

    [JsonPropertyName("Audience")]
    public string Audience { get; set; } = "";
}
