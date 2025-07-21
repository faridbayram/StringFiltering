using System.Collections.Concurrent;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Exceptions;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Infrastructure.Utilities.Storage;

public class InMemoryUploadStorage : IUploadStorage
{
    private readonly IStringArchiver _stringArchiver;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, string>> _allChunks = new(); 
    private readonly ConcurrentDictionary<string, byte[]>  _finalContents = new();

    public InMemoryUploadStorage(IStringArchiver stringArchiver)
    {
        _stringArchiver = stringArchiver;
    }

    public bool IsCompleted(string uploadId) => _finalContents.ContainsKey(uploadId);

    public void StoreChunk(string uploadId, int index, string newChunk)
    {
        var chunksOfGivenUpload = _allChunks.GetOrAdd(uploadId, _ => new ConcurrentDictionary<int, string>());
        chunksOfGivenUpload[index] = newChunk;
    }

    public bool TryAssemble(string uploadId, out string fullText)
    {
        fullText = string.Empty;
        
        if (!_allChunks.TryRemove(uploadId, out var chunksOfCurrentUpload))
            return false;

        var orderedValues = chunksOfCurrentUpload
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => kvp.Value);
        fullText = string.Join("", orderedValues);
        return true;
    }

    public async IAsyncEnumerable<string> StreamUploadChunksAsync(string uploadId)
    {
        var chunksOfCurrentUpload = _allChunks.GetOrAdd(uploadId, _ => new ConcurrentDictionary<int, string>());

        foreach (var chunk in chunksOfCurrentUpload.OrderBy(kvp => kvp.Key))
        {
            await Task.Yield();
            yield return chunk.Value;
        }
    }

    public void StoreFinalText(string uploadId, string finalText)
    {
        var finalContent = _stringArchiver.Archive(finalText);
        _finalContents.AddOrUpdate(uploadId,
            _ => finalContent,
            (_, _) => finalContent);
    }

    public string FetchFinalText(string uploadId)
    {
        if (!_finalContents.TryGetValue(uploadId, out var finalContent))
            throw new BusinessException(
                ClientMessages.UploadNotFound,
                ExceptionMessages.UploadNotFound(uploadId));

        var finalText = _stringArchiver.Unarchive(finalContent);
        return finalText;
    }

    public bool RemoveUpload(string uploadId) => _allChunks.Remove(uploadId, out _);
}