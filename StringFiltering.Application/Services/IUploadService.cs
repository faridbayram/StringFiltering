using StringFiltering.Application.Dtos;
using StringFiltering.Application.Results;

namespace StringFiltering.Application.Services;

public interface IUploadService
{
    bool IsCompleted(string uploadId);
    Task<Result> UploadString(UploadRequestDto requestDto);
    void StoreFinalText(string uploadId, string finalText);
    bool RemoveUpload(string uploadId);
}