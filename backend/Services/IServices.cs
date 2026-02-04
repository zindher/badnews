using BadNews.Models;

namespace BadNews.Services;

public interface ITwilioService
{
    Task<(bool Success, string CallSid)> MakeCallAsync(string toPhoneNumber, string message, int orderId);
    Task<string> GetRecordingAsync(string recordingSid);
    Task<bool> SendSmsAsync(string toPhoneNumber, string message);
    Task<bool> HangupCallAsync(string callSid);
    Task<CallDetails> GetCallDetailsAsync(string callSid);
}

public interface IMercadoPagoService
{
    Task<(bool Success, string PaymentId)> CreatePaymentAsync(int orderId, decimal amount, string buyerEmail, string paymentMethodId);
    Task<(bool Success, string Status)> GetPaymentStatusAsync(string paymentId);
    Task<(bool Success, string RefundId)> RefundPaymentAsync(string paymentId, decimal amount);
}

public interface ISendGridService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
    Task<bool> SendOrderConfirmationAsync(string buyerEmail, string buyerName, int orderId, decimal amount);
    Task<bool> SendPaymentReceiptAsync(string buyerEmail, string buyerName, int orderId, decimal amount, string paymentId);
    Task<bool> SendCallCompletionAsync(string buyerEmail, string buyerName, int orderId, string recordingUrl);
    Task<bool> SendRefundNotificationAsync(string buyerEmail, string buyerName, int orderId, decimal amount, string reason);
    Task<bool> SendMessengerPaymentAsync(string messengerEmail, string messengerName, decimal amount, int callsCompleted);
    Task<bool> SendOrderNotificationAsync(string messengerEmail, string messengerName, int orderId, string recipientPhone, decimal amount);
}

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string toName, string orderId, decimal amount);
    Task SendOrderAcceptedAsync(string buyerEmail, string buyerName, string messengerName, string orderId);
    Task SendPaymentSuccessAsync(string email, string name, string orderId, decimal amount, string paymentId);
    Task SendPaymentFailedAsync(string email, string name, string orderId, string reason);
    Task SendCallReminderAsync(string email, string name, string messengerName, string orderId);
    Task SendEarningsNotificationAsync(string email, string name, decimal earnings, string period);
}

public interface IOrderService
{
    Task<int> CreateOrderAsync(
        string buyerId, 
        string recipientPhone, 
        string recipientName, 
        string message, 
        bool isAnonymous, 
        decimal price, 
        string? preferredCallTime = null, 
        string? recipientTimezone = null, 
        string? recipientState = null,
        string? recipientEmail = null);
    Task AssignOrderAsync(int orderId, string messengerId);
    Task<List<object>> GetAvailableOrdersAsync();
    Task UpdateOrderStatusAsync(int orderId, string status);
}

public interface IMessengerService
{
    Task<bool> CreateMessengerProfileAsync(Guid userId);
    Task SetAvailabilityAsync(Guid messengerId, bool isAvailable);
    Task UpdateEarningsAsync(Guid messengerId, decimal amount);
}
