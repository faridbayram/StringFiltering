using Microsoft.Extensions.Configuration;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Common.Enums;

namespace StringFiltering.Infrastructure.Utilities.Filtering;

// This class is created only for demonstration purposes.
public class JaroWinklerFilteringHelper : BaseFilteringHelper
{
    public JaroWinklerFilteringHelper(IConfiguration configuration)
        : base(
            configuration.GetSection($"{AppConfigKeys.JaroWinkler}:{AppConfigKeys.FilterWords}").Get<string[]>() ?? [],
            ParseThreshold(configuration[$"{AppConfigKeys.JaroWinkler}:{AppConfigKeys.FilteringThreshold}"]))
    {
    }

    public override FilteringStrategy FilteringStrategy => FilteringStrategy.JaroWinkler;

    protected override double Similarity(string firstText, string secondText)
    {
        // Jaro-Winkler similarity logic here. Intentionally left blank. Kindly, do not confuse with ISP violation:)
        throw new NotImplementedException();
    }
}