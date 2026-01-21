using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Services;
using BadNews.Models;
using BadNews.Data;

namespace BadNews.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Messenger")]
public class MessengersController : ControllerBase
{
    private readonly IMessengerService _messengerService;
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<MessengersController> _logger;

    public MessengersController(IMessengerService messengerService, BadNewsDbContext dbContext, ILogger<MessengersController> logger)
    {
        _messengerService = messengerService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMessenger(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var messengerId))
                return BadRequest("Invalid messenger ID");

            var messenger = await _dbContext.Messengers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == messengerId);

            if (messenger == null)
                return NotFound();

            return Ok(new
            {
                success = true,
                data = new
                {
                    messenger.Id,
                    messenger.IsAvailable,
                    messenger.AverageRating,
                    messenger.TotalCompletedOrders,
                    messenger.TotalEarnings,
                    User = new { messenger.User!.FirstName, messenger.User.LastName }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messenger");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPut("{id}/availability")]
    public async Task<IActionResult> SetAvailability(string id, [FromBody] SetAvailabilityRequest request)
    {
        try
        {
            if (!Guid.TryParse(id, out var messengerId))
                return BadRequest("Invalid messenger ID");

            var messenger = await _dbContext.Messengers.FindAsync(messengerId);
            if (messenger == null)
                return NotFound();

            await _messengerService.SetAvailabilityAsync(messengerId, request.IsAvailable);

            return Ok(new
            {
                success = true,
                message = "Availability updated",
                data = new { messenger.IsAvailable }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpGet("{id}/earnings")]
    public async Task<IActionResult> GetEarnings(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var messengerId))
                return BadRequest("Invalid messenger ID");

            var messenger = await _dbContext.Messengers.FindAsync(messengerId);
            if (messenger == null)
                return NotFound();

            return Ok(new
            {
                success = true,
                data = new
                {
                    messenger.TotalEarnings,
                    messenger.PendingBalance,
                    AvailableForWithdraw = messenger.PendingBalance
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting earnings");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> WithdrawEarnings(string id, [FromBody] WithdrawRequest request)
    {
        try
        {
            if (!Guid.TryParse(id, out var messengerId))
                return BadRequest("Invalid messenger ID");

            var messenger = await _dbContext.Messengers.FindAsync(messengerId);
            if (messenger == null)
                return NotFound();

            if (messenger.PendingBalance < request.Amount)
                return BadRequest("Insufficient balance");

            // TODO: Implement actual withdrawal logic (transfer to bank account)
            messenger.PendingBalance -= request.Amount;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Withdrawal of {request.Amount} for messenger {messengerId}");

            return Ok(new
            {
                success = true,
                message = "Withdrawal initiated",
                data = new { newBalance = messenger.PendingBalance }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing earnings");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}

public class SetAvailabilityRequest
{
    public bool IsAvailable { get; set; }
}

public class WithdrawRequest
{
    public decimal Amount { get; set; }
}
