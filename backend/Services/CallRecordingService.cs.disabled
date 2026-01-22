using BadNews.Models;
using BadNews.Data;
using System.Text.Json;

namespace BadNews.Services;

public interface ICallRecordingService
{
    Task<(bool Success, string RecordingUrl)> GetRecordingUrlAsync(string callSid);
    Task<(bool Success, byte[])> DownloadRecordingAsync(string recordingUrl);
    Task<bool> StoreRecordingMetadataAsync(int callAttemptId, string recordingUrl, int durationSeconds);
    Task<List<CallRecording>> GetCallRecordingsAsync(int orderId);
    Task<bool> DeleteRecordingAsync(string recordingSid);
}

public class CallRecordingService : ICallRecordingService
{
    private readonly ITwilioService _twilioService;
    private readonly BadNewsDbContext _dbContext;
    private readonly ILogger<CallRecordingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public CallRecordingService(
        ITwilioService twilioService,
        BadNewsDbContext dbContext,
        ILogger<CallRecordingService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _twilioService = twilioService;
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<(bool Success, string RecordingUrl)> GetRecordingUrlAsync(string callSid)
    {
        try
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];

            // Get recordings for this call
            var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Calls/{callSid}/Recordings.json";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    var root = doc.RootElement;
                    var recordings = root.GetProperty("recordings");
                    
                    if (recordings.GetArrayLength() > 0)
                    {
                        var firstRecording = recordings[0];
                        var recordingSid = firstRecording.GetProperty("sid").GetString();
                        var recordingUrl = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Recordings/{recordingSid}.mp3";
                        
                        _logger.LogInformation($"Recording found: {recordingSid}");
                        return (true, recordingUrl);
                    }
                }
            }

            _logger.LogWarning($"No recordings found for call: {callSid}");
            return (false, "");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting recording URL: {ex.Message}");
            return (false, "");
        }
    }

    public async Task<(bool Success, byte[])> DownloadRecordingAsync(string recordingUrl)
    {
        try
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];

            var request = new HttpRequestMessage(HttpMethod.Get, recordingUrl);
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                _logger.LogInformation($"Recording downloaded, size: {content.Length} bytes");
                return (true, content);
            }

            return (false, null);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error downloading recording: {ex.Message}");
            return (false, null);
        }
    }

    public async Task<bool> StoreRecordingMetadataAsync(int callAttemptId, string recordingUrl, int durationSeconds)
    {
        try
        {
            var callAttempt = await _dbContext.CallAttempts.FindAsync(callAttemptId);
            
            if (callAttempt == null)
            {
                _logger.LogWarning($"CallAttempt not found: {callAttemptId}");
                return false;
            }

            callAttempt.RecordingUrl = recordingUrl;
            callAttempt.RecordingDuration = durationSeconds;
            callAttempt.UpdatedAt = DateTime.UtcNow;

            _dbContext.CallAttempts.Update(callAttempt);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Recording metadata stored for call attempt: {callAttemptId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error storing recording metadata: {ex.Message}");
            return false;
        }
    }

    public async Task<List<CallRecording>> GetCallRecordingsAsync(int orderId)
    {
        try
        {
            var recordings = await _dbContext.CallAttempts
                .Where(ca => ca.Order.Id == orderId && ca.RecordingUrl != null)
                .Select(ca => new CallRecording
                {
                    Id = ca.Id,
                    CallAttemptId = ca.Id,
                    RecordingUrl = ca.RecordingUrl,
                    DurationSeconds = ca.RecordingDuration ?? 0,
                    CreatedAt = ca.CreatedAt,
                    MessengerName = ca.Messenger.Name
                })
                .ToListAsync();

            return recordings;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting call recordings: {ex.Message}");
            return new List<CallRecording>();
        }
    }

    public async Task<bool> DeleteRecordingAsync(string recordingSid)
    {
        try
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];

            var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Recordings/{recordingSid}.json";
            
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
            request.Headers.Add("Authorization", $"Basic {credentials}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Recording deleted: {recordingSid}");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting recording: {ex.Message}");
            return false;
        }
    }
}

public class CallRecording
{
    public int Id { get; set; }
    public int CallAttemptId { get; set; }
    public string RecordingUrl { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MessengerName { get; set; }
}
