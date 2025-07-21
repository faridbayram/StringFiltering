using FluentValidation;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Dtos;
using StringFiltering.Application.Results;
using StringFiltering.Application.Services;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Application.Implementations;

public class UploadService : IUploadService
{
    private readonly IUploadStorage _storage;
    private readonly IBackgroundQueue _filteringQueue;
    private readonly IValidator<UploadRequestDto> _validator;

    public UploadService(
        IUploadStorage storage,
        IBackgroundQueue filteringQueue,
        IValidator<UploadRequestDto> validator)
    {
        _storage = storage;
        _filteringQueue = filteringQueue;
        _validator = validator;
    }

    public bool IsCompleted(string uploadId) => _storage.IsCompleted(uploadId);

    public async Task<Result> UploadString(UploadRequestDto requestDto)
    {
        var validationResult = await _validator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors
                .Select(x => x.ErrorMessage)
                .ToArray();
            var finalMessage = string.Join(Environment.NewLine, errorMessages);

            return Result.Failed(finalMessage);
        }


        if (IsCompleted(requestDto.UploadId))
            return Result.Failed(ClientMessages.ChunksUnavailableForCompletedUpload);

        _storage.StoreChunk(requestDto.UploadId, requestDto.ChunkIndex, requestDto.Data);

        if (!requestDto.IsLastChunk) 
            return Result.Succeeded();

        var chunksStream = _storage.StreamUploadChunksAsync(requestDto.UploadId);
        _filteringQueue.Enqueue(requestDto.UploadId, chunksStream);

        return Result.Succeeded();
    }

    public void StoreFinalText(string uploadId, string finalText) => _storage.StoreFinalText(uploadId, finalText);
    public bool RemoveUpload(string uploadId) => _storage.RemoveUpload(uploadId);
}