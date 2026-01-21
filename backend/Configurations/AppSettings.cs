using System.Text.Json.Serialization;

namespace BadNews.Configurations;

public class AppSettings
{
    [JsonPropertyName("database")]
    public DatabaseSettings Database { get; set; } = new();

    [JsonPropertyName("twilio")]
    public TwilioSettings Twilio { get; set; } = new();

    [JsonPropertyName("mercadoPago")]
    public MercadoPagoSettings MercadoPago { get; set; } = new();

    [JsonPropertyName("sendGrid")]
    public SendGridSettings SendGrid { get; set; } = new();

    [JsonPropertyName("jwt")]
    public JwtSettings Jwt { get; set; } = new();
}

public class DatabaseSettings
{
    [JsonPropertyName("connectionString")]
    public string ConnectionString { get; set; } = "";
}

public class TwilioSettings
{
    [JsonPropertyName("accountSid")]
    public string AccountSid { get; set; } = "";

    [JsonPropertyName("authToken")]
    public string AuthToken { get; set; } = "";

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = "";
}

public class MercadoPagoSettings
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; } = "";
}

public class SendGridSettings
{
    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = "";

    [JsonPropertyName("fromEmail")]
    public string FromEmail { get; set; } = "";
}

public class JwtSettings
{
    [JsonPropertyName("secret")]
    public string Secret { get; set; } = "";

    [JsonPropertyName("expirationMinutes")]
    public int ExpirationMinutes { get; set; } = 60;

    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = "";

    [JsonPropertyName("audience")]
    public string Audience { get; set; } = "";
}
