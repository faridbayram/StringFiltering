namespace StringFiltering.Application.Common.Constants;

public static class ClientMessages
{
    public const string UploadNotFound = "We can not find such an upload. It might be still in progress.";
    public const string ChunksUnavailableForCompletedUpload = "We can not process your chunk, since the upload was already completed.";
    public const string GeneralException = "Oops! Something went wrong. Please try again.";
}