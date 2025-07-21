namespace StringFiltering.Application.Utilities;

public interface IBackgroundQueue
{
    void Enqueue(string uploadId, IAsyncEnumerable<string> chunksStream);
    bool TryDequeue(out string uploadId, out IAsyncEnumerable<string> chunksStream);
}