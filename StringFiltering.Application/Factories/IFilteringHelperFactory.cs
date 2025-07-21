using StringFiltering.Application.Common.Enums;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Application.Factories;

public interface IFilteringHelperFactory
{
    IFilteringHelper GetFilteringHelper(FilteringStrategy filteringStrategy);
}