using Hangfire;
using BadNews.Models;
using BadNews.Configurations;
using BadNews.Data;
using BadNews.Services;

namespace BadNews.Jobs;

public interface ICallRetryJob
{
    Task ExecuteRetryAsync(int orderId);
}

public class CallRetryJob : ICallRetryJob
{
    private readonly BadNewsDbContext _dbContext;
    private readonly ITwilioService _twilioService;
    private readonly ISendGridService _sendGridService;
    private readonly ILogger<CallRetryJob> _logger;
    private readonly IConfiguration _configuration;

    // Retry strategy: 3 calls per day for 3 days
    private readonly int[] _callTimes = { 9, 12, 15 }; // 9 AM, 12 PM, 3 PM
    private const int MaxDays = 3;
    private const int CallsPerDay = 3;

    public CallRetryJob(
        BadNewsDbContext dbContext,
        ITwilioService twilioService,
        ISendGridService sendGridService,
        ILogger<CallRetryJob> logger,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _twilioService = twilioService;
        _sendGridService = sendGridService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Executes a retry attempt for a failed call
    /// Retry Strategy: 3 calls/day × 3 days = 9 total attempts
    /// After 9 failed attempts: SMS fallback + email notification
    /// </summary>
    public async Task ExecuteRetryAsync(int orderId)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Messenger)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning($"Order {orderId} not found for retry");
                return;
            }

            // Initialize first attempt date if not set
            if (!order.FirstCallAttemptDate.HasValue)
            {
                order.FirstCallAttemptDate = DateTime.UtcNow;
            }

            // Check if we've exceeded 9 total attempts (3 days × 3 calls/day)
            if (order.CallAttempts >= MaxDays * CallsPerDay)
            {
                _logger.LogInformation($"Order {orderId} has reached maximum retry attempts ({order.CallAttempts})");
                order.Status = OrderStatus.Failed;
                await HandleMaxRetriesAsync(order);
                await _dbContext.SaveChangesAsync();
                return;
            }

            // Check if order is still pending
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress)
            {
                _logger.LogInformation($"Order {orderId} is not pending (Status: {order.Status}), skipping retry");
                return;
            }

            // Calculate current retry day
            var daysSinceFirstAttempt = (DateTime.UtcNow - order.FirstCallAttemptDate.Value).Days;
            if (daysSinceFirstAttempt >= MaxDays)
            {
                _logger.LogInformation($"Order {orderId} exceeded 3-day retry window");
                order.Status = OrderStatus.Failed;
                await HandleMaxRetriesAsync(order);
                await _dbContext.SaveChangesAsync();
                return;
            }

            // Update retry day if it has changed
            order.RetryDay = daysSinceFirstAttempt;

            _logger.LogInformation($"Executing call attempt {order.CallAttempts + 1}/9 for order {orderId} (Day {order.RetryDay + 1}/{MaxDays}, Daily attempt {order.DailyAttempts + 1}/{CallsPerDay})");

            // Make the call through Twilio
            var (success, callSid) = await _twilioService.MakeCallAsync(
                order.RecipientPhoneNumber,
                order.Message,
                (int)order.Id);

            order.CallAttempts++;
            order.DailyAttempts++;
            order.LastCallAttemptAt = DateTime.UtcNow;

            if (success)
            {
                order.Status = OrderStatus.InProgress;
                _logger.LogInformation($"Call initiated for order {orderId}, Call SID: {callSid}, Attempt {order.CallAttempts}/9");
            }
            else
            {
                _logger.LogWarning($"Call failed for order {orderId}, Attempt {order.CallAttempts}/9");

                // If we haven't reached 9 attempts, schedule the next retry
                if (order.CallAttempts < MaxDays * CallsPerDay)
                {
                    ScheduleNextRetry(order);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error executing retry for order {orderId}");
            throw;
        }
    }

    /// <summary>
    /// Schedules all retries for an order (called when order is first created)
    /// Strategy: 3 calls per day for 3 days at fixed times (9 AM, 12 PM, 3 PM)
    /// </summary>
    public void ScheduleRetries(Order order)
    {
        var now = DateTime.Now;
        var today = now.Date;

        // Schedule retries for next 3 days
        for (int day = 0; day < MaxDays; day++)
        {
            for (int timeIndex = 0; timeIndex < CallsPerDay; timeIndex++)
            {
                var scheduleTime = today.AddDays(day).AddHours(_callTimes[timeIndex]);

                // Only schedule future times
                if (scheduleTime > now)
                {
                    BackgroundJob.Schedule(() => ExecuteRetryAsync((int)order.Id), scheduleTime);
                    _logger.LogInformation($"Scheduled retry {(day * CallsPerDay) + timeIndex + 1}/9 for order {order.Id} at {scheduleTime:yyyy-MM-dd HH:mm} (Day {day + 1}/{MaxDays}, Time {timeIndex + 1}/{CallsPerDay})");
                }
            }
        }
    }

    /// <summary>
    /// Handles when maximum retry attempts are reached (9 attempts failed)
    /// Sends SMS fallback to recipient + email notification to buyer
    /// </summary>
    private async Task HandleMaxRetriesAsync(Order order)
    {
        _logger.LogInformation($"Handling max retries for order {order.Id} - All 9 call attempts failed");

        try
        {
            // Send SMS fallback to recipient
            if (!string.IsNullOrEmpty(order.RecipientPhoneNumber))
            {
                var smsSent = await SendSMSFallbackAsync(order);
                if (smsSent)
                {
                    order.FallbackSMSSent = true;
                }
            }

            // Send email fallback to recipient (if email provided)
            if (!string.IsNullOrEmpty(order.RecipientEmail))
            {
                var emailSent = await SendEmailFallbackAsync(order);
                if (emailSent)
                {
                    order.FallbackEmailSent = true;
                }
            }

            // Notify buyer about failed delivery and refund
            if (!string.IsNullOrEmpty(order.Buyer?.Email))
            {
                await SendBuyerNotificationAsync(order);
            }

            // Process refund to buyer
            await ProcessRefundAsync(order);

            _logger.LogInformation($"Max retries handling completed for order {order.Id} - SMS: {order.FallbackSMSSent}, Email: {order.FallbackEmailSent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling max retries for order {order.Id}");
        }
    }

    /// <summary>
    /// Sends SMS fallback message to recipient
    /// </summary>
    private async Task<bool> SendSMSFallbackAsync(Order order)
    {
        try
        {
            var smsMessage = $"Hola {order.RecipientName ?? ""}! Te tienen un mensaje especial a través de Gritalo. 📢 Contáctanos si necesitas recibirlo por otro medio. ¡Gracias!";
            
            var smsSent = await _twilioService.SendSmsAsync(
                order.RecipientPhoneNumber,
                smsMessage);

            if (smsSent)
            {
                _logger.LogInformation($"SMS fallback sent to {order.RecipientPhoneNumber} for order {order.Id}");
                return true;
            }
            else
            {
                _logger.LogWarning($"Failed to send SMS fallback to {order.RecipientPhoneNumber} for order {order.Id}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending SMS fallback for order {order.Id}");
            return false;
        }
    }

    /// <summary>
    /// Sends email fallback message to recipient (if email provided)
    /// </summary>
    private async Task<bool> SendEmailFallbackAsync(Order order)
    {
        try
        {
            var emailSubject = "¡Tienes un mensaje especial en Gritalo! 📢";
            var emailBody = $@"
            <h2>¡Hola {order.RecipientName ?? ""}!</h2>
            <p>Alguien muy especial te dejó un mensaje a través de Gritalo.</p>
            <p><strong>Infelizmente no pudimos comunicarnos por teléfono</strong>, pero no queremos que te pierdas este regalo.</p>
            
            <p>Para recibir tu mensaje personalizado:</p>
            <ol>
                <li>Visita <a href='https://gritalo.mx'>gritalo.mx</a></li>
                <li>Ingresa tu número de teléfono: {order.RecipientPhoneNumber}</li>
                <li>¡Escucha tu mensaje especial!</li>
            </ol>
            
            <p>Si prefieres que te llamemos de nuevo, puedes indicarnos una mejor hora contactando directamente.</p>
            
            <p>¡No te pierdas este momento! 🎉</p>
            <p style='font-size: 12px; color: #666;'>- Equipo de Gritalo 📢</p>";

            var emailSent = await _sendGridService.SendEmailAsync(
                order.RecipientEmail,
                emailSubject,
                emailBody);

            if (emailSent)
            {
                _logger.LogInformation($"Email fallback sent to {order.RecipientEmail} for order {order.Id}");
                return true;
            }
            else
            {
                _logger.LogWarning($"Failed to send email fallback to {order.RecipientEmail} for order {order.Id}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email fallback for order {order.Id}");
            return false;
        }
    }

    /// <summary>
    /// Sends notification email to buyer about failed delivery
    /// </summary>
    private async Task<bool> SendBuyerNotificationAsync(Order order)
    {
        try
        {
            var emailSubject = "Actualizacion sobre tu orden de Gritalo - Se procesará reembolso";
            var emailBody = $@"
            <h2>Actualización sobre tu Orden de Gritalo</h2>
            <p>Lamentamos informarte que después de 9 intentos de llamada durante 3 días consecutivos, no pudimos contactar al receptor de tu mensaje.</p>
            
            <p><strong>Detalles de la orden:</strong></p>
            <ul>
                <li>Número de teléfono: {order.RecipientPhoneNumber}</li>
                <li>Intentos realizados: {order.CallAttempts}/9</li>
                <li>Período de reintentos: 3 días</li>
                <li>Envío SMS: {'Sí' if order.FallbackSMSSent else 'No'}</li>
                <li>Envío Email: {'Sí' if order.FallbackEmailSent else 'No'}</li>
            </ul>
            
            <p><strong>Reembolso:</strong> Se procesará un reembolso completo de ${order.Price} a tu método de pago original dentro de 24-48 horas.</p>
            
            <p>Si tienes preguntas o necesitas ayuda, contáctanos en support@gritalo.mx</p>
            
            <p>¡Gracias por usar Gritalo! 📢</p>";

            var emailSent = await _sendGridService.SendEmailAsync(
                order.Buyer.Email,
                emailSubject,
                emailBody);

            if (emailSent)
            {
                _logger.LogInformation($"Buyer notification email sent to {order.Buyer.Email} for order {order.Id}");
                return true;
            }
            else
            {
                _logger.LogWarning($"Failed to send buyer notification for order {order.Id}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending buyer notification for order {order.Id}");
            return false;
        }
    }

    /// <summary>
    /// Processes refund for failed delivery
    /// </summary>
    private async Task<bool> ProcessRefundAsync(Order order)
    {
        try
        {
            var payment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.OrderId == order.Id);

            if (payment != null && payment.Status == PaymentStatus.Completed)
            {
                payment.Status = PaymentStatus.Refunded;
                order.Status = OrderStatus.Failed;
                order.PaymentStatus = PaymentStatus.Refunded;
                
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Refund processed for order {order.Id} - Payment ID: {payment.Id}");
                return true;
            }
            else
            {
                _logger.LogWarning($"No completed payment found for order {order.Id} to refund");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing refund for order {order.Id}");
            return false;
        }
    }

    /// <summary>
    /// Schedules the next retry attempt for today/tomorrow
    /// </summary>
    private void ScheduleNextRetry(Order order)
    {
        var now = DateTime.Now;
        var nextRetryTime = FindNextTimeSlot(now);

        BackgroundJob.Schedule(() => ExecuteRetryAsync((int)order.Id), nextRetryTime);
        _logger.LogInformation($"Next retry scheduled for order {order.Id} at {nextRetryTime:yyyy-MM-dd HH:mm} (Attempt {order.CallAttempts + 1}/9)");
    }

    /// <summary>
    /// Finds the next available time slot for a retry (9 AM, 12 PM, 3 PM)
    /// </summary>
    private DateTime FindNextTimeSlot(DateTime currentTime)
    {
        var today = currentTime.Date;

        foreach (var hour in _callTimes)
        {
            var slotTime = today.AddHours(hour);
            if (slotTime > currentTime)
            {
                return slotTime;
            }
        }

        // If no slot available today, schedule for tomorrow at first slot
        return today.AddDays(1).AddHours(_callTimes[0]);
    }
}

