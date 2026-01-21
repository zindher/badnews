namespace BadNews.DTOs;

// Auth DTOs
public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = "Buyer"; // Buyer or Messenger
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}

// Order DTOs
public class CreateOrderDto
{
    public string RecipientPhone { get; set; } = null!;
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set; } // Optional: for email fallback notification
    public string Message { get; set; } = null!;
    public string MessageType { get; set; } = null!; // joke, confession, truth
    public decimal Price { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public string? PreferredCallTime { get; set; } // HH:MM format (Aguascalientes time)
    public string? RecipientState { get; set; } // For timezone mapping
}

public class OrderDetailDto
{
    public int Id { get; set; }
    public string? RecipientName { get; set; }
    public string RecipientPhone { get; set; } = null!;
    public string? RecipientEmail { get; set; }
    public string Message { get; set; } = null!;
    public string MessageType { get; set; } = null!;
    public bool IsAnonymous { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = null!;
    public string? PaymentStatus { get; set; }
    public string? RecordingUrl { get; set; }
    public int? Rating { get; set; }
    public string? Review { get; set; }
    public int CallAttempts { get; set; }
    public int RetryDay { get; set; }
    public int DailyAttempts { get; set; }
    public string? PreferredCallTime { get; set; }
    public string? RecipientTimezone { get; set; }
    public string? RecipientState { get; set; }
    public bool FallbackSMSSent { get; set; }
    public bool FallbackEmailSent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderListItemDto
{
    public int Id { get; set; }
    public string? RecipientName { get; set; }
    public string MessageType { get; set; } = null!;
    public decimal Price { get; set; }
    public string Status { get; set; } = null!;
    public string? PreferredCallTime { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RateOrderDto
{
    public int Rating { get; set; } // 1-5
    public string? Review { get; set; }
}

// Messenger DTOs
public class MessengerProfileDto
{
    public int Id { get; set; }
    public bool IsAvailable { get; set; }
    public decimal Rating { get; set; }
    public int TotalCalls { get; set; }
    public decimal TotalEarnings { get; set; }
    public int MaxCallsPerDay { get; set; }
}

public class UpdateMessengerAvailabilityDto
{
    public bool IsAvailable { get; set; }
    public int? MaxCallsPerDay { get; set; }
}

public class MessengerEarningsDto
{
    public decimal TotalEarnings { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal WithdrawnBalance { get; set; }
    public int CompletedOrders { get; set; }
}

// Payment DTOs
public class CreatePaymentDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethodId { get; set; } = null!;
}

public class PaymentStatusDto
{
    public int Id { get; set; }
    public string? MercadoPagoId { get; set; }
    public string Status { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WithdrawFundsDto
{
    public decimal Amount { get; set; }
    public string BankAccountNumber { get; set; } = null!;
    public string BankCode { get; set; } = null!;
}

// Call DTOs
public class CallAttemptDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? TwilioCallSid { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int Duration { get; set; }
    public string? RecordingUrl { get; set; }
    public int AttemptNumber { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MakeCallDto
{
    public string PhoneNumber { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class CallStatusCallbackDto
{
    public string CallSid { get; set; } = null!;
    public string CallStatus { get; set; } = null!;
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
}

public class RecordingCallbackDto
{
    public string CallSid { get; set; } = null!;
    public string RecordingSid { get; set; } = null!;
    public string RecordingUrl { get; set; } = null!;
}

// Common Response DTOs
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
