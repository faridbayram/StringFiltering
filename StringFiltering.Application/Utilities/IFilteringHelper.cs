using StringFiltering.Application.Common.Enums;

namespace StringFiltering.Application.Utilities;

public interface IFilteringHelper
{
    string Filter(string input);
    FilteringStrategy FilteringStrategy { get; }
}