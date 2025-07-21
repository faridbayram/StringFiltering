using System.Collections.Concurrent;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Common.Enums;
using StringFiltering.Application.Extensions;
using StringFiltering.Application.Factories;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Infrastructure.Factories;

public class FilteringHelperFactory : IFilteringHelperFactory
{
    private readonly ConcurrentDictionary<FilteringStrategy, IFilteringHelper>  _filteringHelpersCache;

    public FilteringHelperFactory(
        IEnumerable<IFilteringHelper>  filteringHelpers)
    {
        _filteringHelpersCache = filteringHelpers.ToConcurrentDictionary(helper => helper.FilteringStrategy);
    }

    public IFilteringHelper GetFilteringHelper(FilteringStrategy filteringStrategy)
    {
        if (!_filteringHelpersCache.TryGetValue(filteringStrategy, out var helper))
            throw new ArgumentOutOfRangeException(ExceptionMessages.FilteringStrategyUnknown(filteringStrategy));

        return helper;
    }
}