using BadNews.Models;
using BadNews.Data;
using System.Text.Json;

namespace BadNews.Services;

public interface IMercadoPagoService
{
    Task<(bool Success, string PaymentId)> CreatePaymentAsync(int orderId, decimal amount, string buyerEmail, string paymentMethodId);
    Task<(bool Success, string Status)> GetPaymentStatusAsync(string paymentId);
    Task<(bool Success, string RefundId)> RefundPaymentAsync(string paymentId, decimal amount);
}

public class MercadoPagoServiceImpl : IMercadoPagoService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MercadoPagoServiceImpl> _logger;
    private readonly BadNewsDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public MercadoPagoServiceImpl(
        IConfiguration configuration,
        ILogger<MercadoPagoServiceImpl> logger,
        BadNewsDbContext dbContext,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task<(bool Success, string PaymentId)> CreatePaymentAsync(
        int orderId,
        decimal amount,
        string buyerEmail,
        string paymentMethodId)
    {
        try
        {
            _logger.LogInformation($"Creating payment for order {orderId}, amount: {amount}");

            var accessToken = _configuration["MercadoPago:AccessToken"];
            
            var paymentData = new
            {
                transaction_amount = (double)amount,
                description = $"BadNews - Orden #{orderId}",
                payment_method_id = paymentMethodId,
                payer = new
                {
                    email = buyerEmail
                },
                metadata = new
                {
                    order_id = orderId
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadopago.com/v1/payments");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(paymentData),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseData))
                {
                    var root = doc.RootElement;
                    var paymentId = root.GetProperty("id").GetString();

                    // Save payment record
                    var payment = new Payment
                    {
                        OrderId = orderId,
                        Amount = amount,
                        ExternalPaymentId = paymentId,
                        Status = "pending",
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Payments.Add(payment);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Payment created successfully: {paymentId}");
                    return (true, paymentId);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"MercadoPago API error: {errorContent}");
            return (false, "");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating payment: {ex.Message}");
            return (false, "");
        }
    }

    public async Task<(bool Success, string Status)> GetPaymentStatusAsync(string paymentId)
    {
        try
        {
            var accessToken = _configuration["MercadoPago:AccessToken"];

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadopago.com/v1/payments/{paymentId}");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseData))
                {
                    var root = doc.RootElement;
                    var status = root.GetProperty("status").GetString();
                    return (true, status ?? "unknown");
                }
            }

            return (false, "error");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting payment status: {ex.Message}");
            return (false, "error");
        }
    }

    public async Task<(bool Success, string RefundId)> RefundPaymentAsync(string paymentId, decimal amount)
    {
        try
        {
            var accessToken = _configuration["MercadoPago:AccessToken"];

            var refundData = new
            {
                amount = (double)amount
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.mercadopago.com/v1/payments/{paymentId}/refunds");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(refundData),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseData))
                {
                    var root = doc.RootElement;
                    var refundId = root.GetProperty("id").GetString();
                    _logger.LogInformation($"Refund created: {refundId}");
                    return (true, refundId ?? "");
                }
            }

            return (false, "");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error refunding payment: {ex.Message}");
            return (false, "");
        }
    }
}
