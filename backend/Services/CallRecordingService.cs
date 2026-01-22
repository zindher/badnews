using BadNews.Models;
using System.Threading.Tasks;

namespace BadNews.Services;

public interface ICallRecordingService
{
    Task<(bool Success, string RecordingUrl)> GetRecordingUrlAsync(string callSid);
}

public class CallRecordingService : ICallRecordingService
{
    public async Task<(bool Success, string RecordingUrl)> GetRecordingUrlAsync(string callSid)
    {
        return (false, "");
    }
}
