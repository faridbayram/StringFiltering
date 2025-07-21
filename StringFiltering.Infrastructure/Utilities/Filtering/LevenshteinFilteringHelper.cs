using Microsoft.Extensions.Configuration;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Common.Enums;

namespace StringFiltering.Infrastructure.Utilities.Filtering;

public class LevenshteinFilteringHelper : BaseFilteringHelper
{
    public LevenshteinFilteringHelper(IConfiguration configuration)
        : base(
            configuration.GetSection($"{AppConfigKeys.Levenshtein}:{AppConfigKeys.FilterWords}").Get<string[]>() ?? [],
            ParseThreshold(configuration[$"{AppConfigKeys.Levenshtein}:{AppConfigKeys.FilteringThreshold}"]))
    {
    }

    public override FilteringStrategy FilteringStrategy => FilteringStrategy.Levenshtein;

    protected override double Similarity(string firstText, string secondText)
    {
        var distance = LevenshteinDistance(firstText, secondText);
        var similarity = 1.0 - (double)distance / Math.Max(firstText.Length, secondText.Length);

        return similarity;
    }

    private static int LevenshteinDistance(string firstText, string secondText)
    {
        var editDistances = new int[firstText.Length + 1, secondText.Length + 1];

        for (var i = 0; i <= firstText.Length; i++)
        {
            editDistances[i, 0] = i;
        }

        for (var j = 0; j <= secondText.Length; j++)
        {
            editDistances[0, j] = j;
        }

        for (var i = 1; i <= firstText.Length; i++)
        {
            for (var j = 1; j <= secondText.Length; j++)
            {
                var cost = firstText[i - 1] == secondText[j - 1] ? 0 : 1;
                editDistances[i, j] = Math.Min(
                    Math.Min(
                        editDistances[i - 1, j] + 1,     // deletion
                        editDistances[i, j - 1] + 1),    // insertion
                    editDistances[i - 1, j - 1] + cost); // substitution
            }
        }

        return editDistances[firstText.Length, secondText.Length];
    }
}