using System.Text;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Common.Enums;
using StringFiltering.Application.Factories;
using StringFiltering.Application.Services;
using StringFiltering.Application.Utilities;

namespace StringFiltering.API.BackgroundServices;

public class FilteringService : BackgroundService
{
    private readonly IFilteringHelperFactory _filteringHelperFactory;
    private readonly IBackgroundQueue _filteringQueue;
    private readonly IUploadService _uploadService;
    private readonly ILogger<FilteringService> _logger;

    public FilteringService(
        IFilteringHelperFactory filteringHelperFactory,
        IBackgroundQueue filteringQueue,
        IUploadService uploadService,
        ILogger<FilteringService> logger)
    {
        _filteringHelperFactory = filteringHelperFactory;
        _filteringQueue = filteringQueue;
        _uploadService = uploadService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // this filtering helper resolution intentionally made here, in order to emphasize dynamic filtering strategy support.
        var filteringHelper = _filteringHelperFactory.GetFilteringHelper(FilteringStrategy.Levenshtein);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_filteringQueue.TryDequeue(out var uploadId, out var chunksStream))
                {
                    var sb = new StringBuilder();
                    var previousLastWord = string.Empty;
                    await foreach (var chunk in chunksStream.WithCancellation(stoppingToken))
                    {
                        var inputToBeFiltered = $"{previousLastWord}{chunk}";

                        var lastWordStartIndex = GetLastWordStartIndex(inputToBeFiltered);
                        if (lastWordStartIndex == -1) // input is empty string
                            continue;
                        if (lastWordStartIndex == 0) // there is only one word in input
                        {
                            previousLastWord = inputToBeFiltered;
                            continue;
                        }

                        previousLastWord = inputToBeFiltered[lastWordStartIndex..];
                        inputToBeFiltered = inputToBeFiltered[..lastWordStartIndex];
                        var filteredChunk = filteringHelper.Filter(inputToBeFiltered);
                        sb.Append(filteredChunk);
                    }

                    // handling last word of last chunk
                    var filteredLastWord = filteringHelper.Filter(previousLastWord);
                    sb.Append(filteredLastWord);

                    _uploadService.StoreFinalText(uploadId, sb.ToString());
                    _logger.LogInformation(LogMessageTemplates.UploadCompletedSuccessfully, uploadId);

                    _uploadService.RemoveUpload(uploadId);
                }
                else
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LogMessageTemplates.GeneralException, ex.Message);
            }
        }
    }

    private static int GetLastWordStartIndex(string chunk)
    {
        if (string.IsNullOrWhiteSpace(chunk))
            return -1;

        chunk = chunk.TrimEnd();

        var lastSpace = chunk.LastIndexOf(' ');
        return lastSpace == -1 ? 0 : lastSpace + 1;
    }
}