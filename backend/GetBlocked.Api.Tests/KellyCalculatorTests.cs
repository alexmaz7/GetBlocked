using GetBlocked.Api.Services;
using Xunit;

namespace GetBlocked.Api.Tests;

public class KellyCalculatorTests
{
    [Fact]
    public void Calculate_PositiveEdge_ReturnsPositiveStake()
    {
        // True prob 55%, decimal odds 2.0 (even money), $1000 bankroll, quarter Kelly
        var result = KellyCalculator.Calculate(0.55, 2.0, 1000, 0.25);

        Assert.True(result.FullKelly > 0);
        Assert.True(result.FractionalKelly > 0);
        Assert.True(result.RecommendedStake > 0);
        Assert.True(result.ExpectedValue > 0);
    }

    [Fact]
    public void Calculate_NoEdge_ReturnsZeroStake()
    {
        // True prob 50%, decimal odds 2.0 — no edge
        var result = KellyCalculator.Calculate(0.50, 2.0, 1000, 0.25);

        Assert.Equal(0, result.FullKelly);
        Assert.Equal(0, result.RecommendedStake);
    }

    [Fact]
    public void Calculate_NegativeEdge_ReturnsZeroStake()
    {
        // True prob 40%, decimal odds 2.0 — negative edge
        var result = KellyCalculator.Calculate(0.40, 2.0, 1000, 0.25);

        Assert.Equal(0, result.FullKelly);
        Assert.Equal(0, result.RecommendedStake);
    }

    [Fact]
    public void Calculate_FractionalKelly_ReducesStake()
    {
        var full = KellyCalculator.Calculate(0.60, 2.0, 1000, 1.0);
        var quarter = KellyCalculator.Calculate(0.60, 2.0, 1000, 0.25);

        Assert.True(quarter.RecommendedStake < full.RecommendedStake);
        Assert.Equal(full.FullKelly, quarter.FullKelly); // full Kelly is same
    }

    [Fact]
    public void Calculate_HighProbability_ClampsToMax()
    {
        // 99% true prob, odds barely above 1 — Kelly wants to bet everything
        var result = KellyCalculator.Calculate(0.99, 1.05, 1000, 1.0);

        Assert.True(result.FullKelly <= 1.0);
    }

    [Fact]
    public void Calculate_KnownValues()
    {
        // True prob 60%, odds +150 (decimal 2.5), $1000 bankroll, full Kelly
        // Kelly = (1.5 * 0.6 - 0.4) / 1.5 = (0.9 - 0.4) / 1.5 = 0.3333
        var result = KellyCalculator.Calculate(0.60, 2.5, 1000, 1.0);

        Assert.Equal(0.3333, result.FullKelly, 3);
        Assert.Equal(333.33, result.RecommendedStake, 0);
    }
}
