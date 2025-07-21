namespace StringFiltering.Application.Utilities;

public interface IUploadStorage
{
    bool IsCompleted(string uploadId);
    void StoreChunk(string uploadId, int index, string newChunk);
    bool TryAssemble(string uploadId, out string fullText);
    IAsyncEnumerable<string> StreamUploadChunksAsync(string uploadId);
    void StoreFinalText(string uploadId, string finalText);
    string FetchFinalText(string uploadId);
    bool RemoveUpload(string uploadId);
}