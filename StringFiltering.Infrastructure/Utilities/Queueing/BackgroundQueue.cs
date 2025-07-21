using System.Collections.Concurrent;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Infrastructure.Utilities.Queueing;

public class BackgroundQueue : IBackgroundQueue
{
    private readonly ConcurrentQueue<KeyValuePair<string, IAsyncEnumerable<string>>> _queue = new();

    public void Enqueue(string uploadId, IAsyncEnumerable<string> chunksStream)
    {
        _queue.Enqueue(new KeyValuePair<string, IAsyncEnumerable<string>>(uploadId, chunksStream));
    }

    public bool TryDequeue(out string uploadId, out IAsyncEnumerable<string> chunksStream)
    {
        var result = _queue.TryDequeue(out var element);
        
        uploadId = element.Key;
        chunksStream = element.Value;

        return result;
    }
}