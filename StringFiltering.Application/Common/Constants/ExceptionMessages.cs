using StringFiltering.Application.Common.Enums;

namespace StringFiltering.Application.Common.Constants;

public static class ExceptionMessages
{
    public const string FilteringThresholdMissing =  "Filtering threshold is missing in configuration.";
    public const string FilteringThresholdInvalid =  "Filtering threshold is invalid.";

    public static string UploadNotFound(string uploadId)
    {
        return $"Upload with id: \"{uploadId}\" is not found in final contents store.";
    }

    public static string FilteringStrategyUnknown(FilteringStrategy filteringStrategy)
    {
        return $"Unknown filtering strategy: \"{filteringStrategy}\".";
    }
}