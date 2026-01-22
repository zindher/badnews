using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using BadNews.Models;
using BadNews.Data;

namespace BadNews.Services;

public class TwilioServiceImpl : ITwilioService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwilioServiceImpl> _logger;
    private readonly BadNewsDbContext _dbContext;

    public TwilioServiceImpl(
        IConfiguration configuration,
        ILogger<TwilioServiceImpl> logger,
        BadNewsDbContext dbContext)
    {
        _configuration = configuration;
        _logger = logger;
        _dbContext = dbContext;

        // Initialize Twilio
        var accountSid = configuration["Twilio:AccountSid"];
        var authToken = configuration["Twilio:AuthToken"];

        if (!string.IsNullOrEmpty(accountSid) && !string.IsNullOrEmpty(authToken))
        {
            TwilioClient.Init(accountSid, authToken);
        }
    }

    /// <summary>
    /// Makes an outbound call with message recording
    /// </summary>
    public async Task<(bool Success, string CallSid)> MakeCallAsync(
        string toPhoneNumber,
        string message,
        int orderId)
    {
        try
        {
            var phoneNumber = _configuration["Twilio:PhoneNumber"]
                ?? throw new InvalidOperationException("Twilio phone number not configured");
            var baseUrl = _configuration["BaseUrl"]
                ?? throw new InvalidOperationException("BaseUrl not configured");

            // Generate TwiML with recording
            var twiml = GenerateTwiML(message, orderId);

            _logger.LogInformation($"Making call to {toPhoneNumber} for order {orderId}");

            // Create the call
            var call = await CallResource.CreateAsync(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(phoneNumber),
                url: new Uri($"{baseUrl}/api/calls/twiml/{orderId}"),
                statusCallbackUrl: new Uri($"{baseUrl}/api/calls/status-callback"),
                statusCallbackEvent: new List<string> 
                { 
                    "initiated", 
                    "ringing", 
                    "answered", 
                    "completed",
                    "failed",
                    "no-answer"
                },
                statusCallbackMethod: Twilio.Http.HttpMethod.Post,
                record: true,
                recordingChannels: "mono",
                recordingStatusCallback: new Uri($"{baseUrl}/api/calls/recording-callback"),
                recordingStatusCallbackMethod: Twilio.Http.HttpMethod.Post
            );

            _logger.LogInformation($"Call initiated: {call.Sid} to {toPhoneNumber}");

            // Log the call attempt in database
            var callAttempt = new Models.CallAttempt
            {
                OrderId = orderId,
                TwilioCallSid = call.Sid,
                PhoneNumber = toPhoneNumber,
                Status = Models.CallStatus.Initiated,
                AttemptNumber = 1,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.CallAttempts.Add(callAttempt);
            await _dbContext.SaveChangesAsync();

            return (true, call.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error making call to {toPhoneNumber}");
            return (false, string.Empty);
        }
    }

    /// <summary>
    /// Generates TwiML for call with recording
    /// </summary>
    private string GenerateTwiML(string message, int orderId)
    {
        var response = new VoiceResponse();

        // Gather input first
        var gather = new Gather(numDigits: 1, action: new Uri($"/api/calls/gather-response/{orderId}"));
        gather.Say(message, voice: "alice", language: "es-MX");
        response.Append(gather);

        // If no input, record message
        response.Record(maxLength: 300, finishOnKey: "#");

        // Hang up
        response.Hangup();

        return response.ToString();
    }

    /// <summary>
    /// Gets recording URL after call completes
    /// </summary>
    public async Task<string> GetRecordingAsync(string recordingSid)
    {
        try
        {
            var recording = await RecordingResource.FetchAsync(pathSid: recordingSid);

            if (recording == null)
            {
                _logger.LogWarning($"Recording not found: {recordingSid}");
                return string.Empty;
            }

            // Construct URL to MP3 file
            var recordingUrl = $"https://api.twilio.com/2010-04-01/Accounts/{recording.AccountSid}/Recordings/{recordingSid}.mp3";

            _logger.LogInformation($"Recording URL: {recordingUrl}");

            return recordingUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting recording: {recordingSid}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Sends SMS as fallback notification
    /// </summary>
    public async Task<bool> SendSmsAsync(string toPhoneNumber, string message)
    {
        try
        {
            var phoneNumber = _configuration["Twilio:PhoneNumber"]
                ?? throw new InvalidOperationException("Twilio phone number not configured");

            var msg = await MessageResource.CreateAsync(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(phoneNumber),
                body: message
            );

            _logger.LogInformation($"SMS sent: {msg.Sid} to {toPhoneNumber}");

            return !string.IsNullOrEmpty(msg.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending SMS to {toPhoneNumber}");
            return false;
        }
    }

    /// <summary>
    /// Hangs up a call
    /// </summary>
    public async Task<bool> HangupCallAsync(string callSid)
    {
        try
        {
            var updated = await CallResource.UpdateAsync(
                pathSid: callSid,
                status: CallResource.UpdateStatus.Completed
            );

            _logger.LogInformation($"Call hung up: {callSid}");
            return updated.Status == CallResource.CallStatus.Completed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error hanging up call: {callSid}");
            return false;
        }
    }

    /// <summary>
    /// Gets call details
    /// </summary>
    public async Task<CallDetails> GetCallDetailsAsync(string callSid)
    {
        try
        {
            var call = await CallResource.FetchAsync(pathSid: callSid);

            if (call == null)
            {
                _logger.LogWarning($"Call not found: {callSid}");
                return null;
            }

            return new CallDetails
            {
                CallSid = call.Sid,
                Status = call.Status?.ToString() ?? "unknown",
                From = call.From,
                To = call.To,
                Duration = call.Duration ?? 0,
                StartTime = call.StartTime,
                EndTime = call.EndTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting call details: {callSid}");
            return null;
        }
    }
}

public class CallDetails
{
    public string CallSid { get; set; }
    public string Status { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public int Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
