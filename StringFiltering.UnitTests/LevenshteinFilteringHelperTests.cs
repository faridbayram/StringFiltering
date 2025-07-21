using Microsoft.Extensions.Configuration;
using StringFiltering.Application.Common.Constants;
using StringFiltering.Application.Common.Enums;
using StringFiltering.Infrastructure.Utilities.Filtering;

namespace StringFiltering.UnitTests;

public class LevenshteinFilteringHelperTests
{
    [Fact]
    public void Filter_RemovesWords_AboveThreshold()
    {
        // Arrange
        var config = CreateTestConfiguration(0.8, ["super", "monkey"]);
        var helper = new LevenshteinFilteringHelper(config);

        // Act
        var result = helper.Filter("supper donkey");

        // Assert
        Assert.Equal(" ", result);
    }

    [Fact]
    public void Filter_RemovesWords_AtThreshold()
    {
        // Arrange
        var config = CreateTestConfiguration(0.8, ["house", "cream"]);
        var helper = new LevenshteinFilteringHelper(config);

        // Act
        var result = helper.Filter("horse dream");

        // Assert
        Assert.Equal(" ", result);
    }

    [Fact]
    public void Filter_KeepsWords_BelowThreshold()
    {
        // Arrange
        var config = CreateTestConfiguration(0.4, ["dog", "cat"]);
        var helper = new LevenshteinFilteringHelper(config);

        // Act
        var result = helper.Filter("bag pet fox");

        // Assert
        Assert.Equal("bag pet fox", result);
    }

    [Fact]
    public void Filter_RemovesExactMatches_WhenThresholdIsOne()
    {
        // Arrange
        var config = CreateTestConfiguration(1.0, ["dog", "cat"]);
        var helper = new LevenshteinFilteringHelper(config);

        // Act
        var result = helper.Filter("dog cat mouse");

        // Assert
        Assert.Equal("  mouse", result);
    }

    [Fact]
    public void Filter_ReturnsEmpty_IfInputIsEmpty()
    {
        // Arrange
        var config = CreateTestConfiguration(0.5, ["word"]);
        var helper = new LevenshteinFilteringHelper(config);

        // Act
        var result = helper.Filter("");

        // Assert
        Assert.Equal(string.Empty, result);
    }

    private static IConfiguration CreateTestConfiguration(double threshold, string[] filterWords)
    {
        var settings = new Dictionary<string, string>();

        settings[$"{AppConfigKeys.Levenshtein}:{AppConfigKeys.FilteringThreshold}"] = threshold.ToString();

        for (var i = 0; i < filterWords.Length; i++) {
            settings[$"{nameof(FilteringStrategy.Levenshtein)}:{AppConfigKeys.FilterWords}:{i}"] = filterWords[i];
        }

        return new ConfigurationBuilder().AddInMemoryCollection(settings!).Build();
    }
}