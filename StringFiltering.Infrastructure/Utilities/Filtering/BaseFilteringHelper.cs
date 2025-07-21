using System.Text;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Common.Enums;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Infrastructure.Utilities.Filtering;

public abstract class BaseFilteringHelper : IFilteringHelper
{
    private readonly string[] _filterWords;
    private readonly double _filteringThreshold;

    public abstract FilteringStrategy FilteringStrategy { get; }

    protected BaseFilteringHelper(string[] filterWords, double filteringThreshold)
    {
        _filterWords = filterWords;
        _filteringThreshold = filteringThreshold;
    }

    protected abstract double Similarity(string firstText, string secondText);

    public string Filter(string input)
    {
        // this algorithm is chosen for 2 purposes:
        // 1 - being able to detect hidden filter words - the ones that spread across chunks.
        // 2 - being able to preserve the layout.

        var output = new StringBuilder(input.Length);
        var wordBuf = new StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (char.IsWhiteSpace(c))
            {
                if (wordBuf.Length > 0)
                {
                    var word = wordBuf.ToString();

                    var isFiltered = _filterWords.Any(fw => Similarity(word, fw) >= _filteringThreshold);
                    if (!isFiltered)
                        output.Append(word);

                    wordBuf.Clear();
                }

                output.Append(c); // preserving the layout here.
            }
            else
            {
                wordBuf.Append(c);
            }
        }

        // Handle final word
        if (wordBuf.Length > 0)
        {
            var word = wordBuf.ToString();
            if (!_filterWords.Any(fw => Similarity(word, fw) >= _filteringThreshold))
                output.Append(word);
        }

        return output.ToString();
    }

    protected static double ParseThreshold(string? thresholdStr)
    {
        if (string.IsNullOrWhiteSpace(thresholdStr) ||
            !double.TryParse(thresholdStr, out var threshold))
        {
            throw new InvalidOperationException(ExceptionMessages.FilteringThresholdInvalid);
        }

        return threshold;
    }
}