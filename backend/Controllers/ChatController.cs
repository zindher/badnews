using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BadNews.Data;
using BadNews.Models;
using System.Security.Claims;

namespace BadNews.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<ChatController> _logger;

    public ChatController(BadNewsDbContext dbContext, ILogger<ChatController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("conversations/{orderId}")]
    public async Task<IActionResult> GetConversation(int orderId)
    {
        try
        {
            var order = await _dbContext.Orders
                .Include(o => o.Buyer)
                .Include(o => o.AcceptedMessenger)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound(new { error = "Order not found" });

            // Check if user is part of this order
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (order.BuyerId.ToString() != userId && order.AcceptedMessengerId.ToString() != userId)
                return Unauthorized();

            var messages = await _dbContext.Messages
                .Where(m => m.OrderId == orderId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.SenderId,
                    SenderName = m.Sender.Name,
                    m.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                orderId,
                buyerName = order.Buyer.Name,
                messengerName = order.AcceptedMessenger?.Name,
                messages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting conversation: {ex.Message}");
            return BadRequest(new { error = "Error loading conversation" });
        }
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var senderId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var order = await _dbContext.Orders.FindAsync(request.OrderId);
            if (order == null)
                return NotFound(new { error = "Order not found" });

            // Check if user is part of this order
            if (order.BuyerId != senderId && order.AcceptedMessengerId != senderId)
                return Unauthorized();

            var message = new Message
            {
                OrderId = request.OrderId,
                SenderId = senderId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message.Id,
                message.Content,
                message.SenderId,
                message.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending message: {ex.Message}");
            return BadRequest(new { error = "Error sending message" });
        }
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetUserConversations()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var conversations = await _dbContext.Orders
                .Where(o => o.BuyerId == userId || o.AcceptedMessengerId == userId)
                .Include(o => o.Buyer)
                .Include(o => o.AcceptedMessenger)
                .Select(o => new
                {
                    o.Id,
                    o.Message,
                    o.Status,
                    OtherPartyName = o.BuyerId == userId ? o.AcceptedMessenger.Name : o.Buyer.Name,
                    LastMessageAt = _dbContext.Messages
                        .Where(m => m.OrderId == o.Id)
                        .Max(m => m.CreatedAt),
                    UnreadCount = _dbContext.Messages
                        .Count(m => m.OrderId == o.Id && m.SenderId != userId && !m.IsRead)
                })
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            return Ok(conversations);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting conversations: {ex.Message}");
            return BadRequest(new { error = "Error loading conversations" });
        }
    }

    [HttpPut("messages/{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead(int messageId)
    {
        try
        {
            var message = await _dbContext.Messages.FindAsync(messageId);
            if (message == null)
                return NotFound(new { error = "Message not found" });

            message.IsRead = true;
            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Message marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error marking message as read: {ex.Message}");
            return BadRequest(new { error = "Error updating message" });
        }
    }

    [HttpPost("conversations/{orderId}/close")]
    public async Task<IActionResult> CloseConversation(int orderId)
    {
        try
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound(new { error = "Order not found" });

            // Check authorization
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (order.BuyerId != userId && order.AcceptedMessengerId != userId)
                return Unauthorized();

            order.Status = "completed";
            order.UpdatedAt = DateTime.UtcNow;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Conversation closed" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error closing conversation: {ex.Message}");
            return BadRequest(new { error = "Error closing conversation" });
        }
    }
}

public class SendMessageRequest
{
    public int OrderId { get; set; }
    public string Content { get; set; }
}

public class Message
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public Order Order { get; set; }
    public User Sender { get; set; }
}
