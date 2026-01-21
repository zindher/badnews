using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Services;
using BadNews.Models;
using BadNews.Data;

namespace BadNews.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CallsController : ControllerBase
{
    private readonly ITwilioService _twilioService;
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<CallsController> _logger;

    public CallsController(ITwilioService twilioService, BadNewsDbContext dbContext, ILogger<CallsController> logger)
    {
        _twilioService = twilioService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [Authorize(Roles = "Messenger")]
    [HttpPost("make-call")]
    public async Task<IActionResult> MakeCall([FromBody] MakeCallRequest request)
    {
        try
        {
            if (!Guid.TryParse(request.OrderId, out var orderId))
                return BadRequest("Invalid order ID");

            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found");

            if (order.Status != OrderStatus.Assigned)
                return BadRequest("Order is not in assigned status");

            // Make the call
            var callSid = await _twilioService.MakeCallAsync(
                order.RecipientPhoneNumber,
                order.Message,
                orderId
            );

            // Record the call attempt
            var callAttempt = new CallAttempt
            {
                OrderId = orderId,
                AttemptNumber = order.CallAttempts + 1,
                Status = CallStatus.Queued,
                TwilioCallSid = callSid
            };

            _dbContext.CallAttempts.Add(callAttempt);
            order.CallAttempts++;
            order.Status = OrderStatus.InProgress;
            order.LastCallAttemptAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Call initiated for order {orderId} - SID: {callSid}");

            return Ok(new
            {
                success = true,
                data = new { callSid, orderId }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making call");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [AllowAnonymous]
    [HttpPost("status-callback")]
    public async Task<IActionResult> CallStatusCallback([FromForm] CallStatusCallbackRequest request)
    {
        try
        {
            _logger.LogInformation($"Call status update - SID: {request.CallSid}, Status: {request.CallStatus}");

            var callAttempt = await _dbContext.CallAttempts
                .FirstOrDefaultAsync(ca => ca.TwilioCallSid == request.CallSid);

            if (callAttempt == null)
                return NotFound();

            // Update call status
            callAttempt.Status = request.CallStatus switch
            {
                "ringing" => CallStatus.Ringing,
                "in-progress" => CallStatus.InProgress,
                "completed" => CallStatus.Completed,
                "failed" => CallStatus.Failed,
                "no-answer" => CallStatus.NoAnswer,
                "busy" => CallStatus.Busy,
                _ => CallStatus.Queued
            };

            // Extract duration if available
            if (int.TryParse(request.Duration, out var duration))
                callAttempt.DurationSeconds = duration;

            // Update connected status
            if (callAttempt.Status == CallStatus.InProgress)
            {
                var order = await _dbContext.Orders.FindAsync(callAttempt.OrderId);
                if (order != null)
                    order.CallConnected = true;
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling call status callback");
            return StatusCode(500);
        }
    }

    [AllowAnonymous]
    [HttpPost("recording-callback")]
    public async Task<IActionResult> RecordingCallback([FromForm] RecordingCallbackRequest request)
    {
        try
        {
            _logger.LogInformation($"Recording received - SID: {request.CallSid}, URL: {request.RecordingUrl}");

            var callAttempt = await _dbContext.CallAttempts
                .FirstOrDefaultAsync(ca => ca.TwilioCallSid == request.CallSid);

            if (callAttempt == null)
                return NotFound();

            callAttempt.RecordingUrl = request.RecordingUrl;

            var order = await _dbContext.Orders.FindAsync(callAttempt.OrderId);
            if (order != null)
            {
                order.CallRecordingUrl = request.RecordingUrl;
                order.Status = OrderStatus.Completed;
                order.CompletedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Order {callAttempt.OrderId} completed with recording");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling recording callback");
            return StatusCode(500);
        }
    }

    [Authorize]
    [HttpGet("{orderId}/attempts")]
    public async Task<IActionResult> GetCallAttempts(Guid orderId)
    {
        try
        {
            var attempts = await _dbContext.CallAttempts
                .Where(ca => ca.OrderId == orderId)
                .OrderByDescending(ca => ca.AttemptedAt)
                .Select(ca => new
                {
                    ca.Id,
                    ca.AttemptNumber,
                    ca.Status,
                    ca.AttemptedAt,
                    ca.DurationSeconds,
                    ca.RecordingUrl
                })
                .ToListAsync();

            return Ok(new { success = true, data = attempts });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching call attempts");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}

public class MakeCallRequest
{
    public string OrderId { get; set; } = null!;
}

public class CallStatusCallbackRequest
{
    public string CallSid { get; set; } = null!;
    public string CallStatus { get; set; } = null!;
    public string? Duration { get; set; }
}

public class RecordingCallbackRequest
{
    public string CallSid { get; set; } = null!;
    public string RecordingUrl { get; set; } = null!;
}
