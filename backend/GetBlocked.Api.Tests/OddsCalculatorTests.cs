using GetBlocked.Api.Services;
using Xunit;

namespace GetBlocked.Api.Tests;

public class OddsCalculatorTests
{
    [Theory]
    [InlineData(-110, 1.909)]
    [InlineData(100, 2.0)]
    [InlineData(-200, 1.5)]
    [InlineData(200, 3.0)]
    [InlineData(-150, 1.667)]
    [InlineData(150, 2.5)]
    public void AmericanToDecimal_ConvertsCorrectly(int american, double expectedDecimal)
    {
        var result = OddsCalculator.AmericanToDecimal(american);
        Assert.Equal(expectedDecimal, result, 2);
    }

    [Theory]
    [InlineData(2.0, 100)]
    [InlineData(3.0, 200)]
    [InlineData(1.5, -200)]
    [InlineData(2.5, 150)]
    public void DecimalToAmerican_ConvertsCorrectly(double decimalOdds, int expectedAmerican)
    {
        var result = OddsCalculator.DecimalToAmerican(decimalOdds);
        Assert.Equal(expectedAmerican, result);
    }

    [Theory]
    [InlineData(2.0, 0.5)]
    [InlineData(3.0, 0.3333)]
    [InlineData(1.5, 0.6667)]
    [InlineData(4.0, 0.25)]
    public void ImpliedProbability_CalculatesCorrectly(double decimalOdds, double expected)
    {
        var result = OddsCalculator.ImpliedProbability(decimalOdds);
        Assert.Equal(expected, result, 3);
    }

    [Fact]
    public void ImpliedProbability_OddsAtOrBelow1_Returns1()
    {
        Assert.Equal(1, OddsCalculator.ImpliedProbability(1));
        Assert.Equal(1, OddsCalculator.ImpliedProbability(0.5));
    }

    [Fact]
    public void RemoveVig_NormalizesToOne()
    {
        var probs = new[] { 0.55, 0.55 }; // 110% total (10% vig)
        var result = OddsCalculator.RemoveVig(probs);
        Assert.Equal(1.0, result.Sum(), 4);
        Assert.Equal(0.5, result[0], 4);
        Assert.Equal(0.5, result[1], 4);
    }

    [Fact]
    public void RemoveVig_ThreeWay_NormalizesToOne()
    {
        var probs = new[] { 0.40, 0.35, 0.35 }; // 110% total
        var result = OddsCalculator.RemoveVig(probs);
        Assert.Equal(1.0, result.Sum(), 4);
    }

    [Fact]
    public void CalculateOverround_SumsCorrectly()
    {
        var probs = new[] { 0.5238, 0.5238 }; // typical -110/-110
        var result = OddsCalculator.CalculateOverround(probs);
        Assert.True(result > 1);
    }

    [Fact]
    public void PowerDevig_TwoOutcome_SumsToOne()
    {
        var probs = new[] { 0.5238, 0.5238 };
        var result = OddsCalculator.PowerDevig(probs);
        Assert.Equal(1.0, result.Sum(), 3);
    }

    [Fact]
    public void PowerDevig_FallsBackToRemoveVig_ForThreeOutcomes()
    {
        var probs = new[] { 0.40, 0.35, 0.35 };
        var result = OddsCalculator.PowerDevig(probs);
        Assert.Equal(1.0, result.Sum(), 4);
    }

    [Fact]
    public void AmericanToImpliedProbability_Positive()
    {
        // +200 => decimal 3.0 => implied 33.33%
        var result = OddsCalculator.AmericanToImpliedProbability(200);
        Assert.Equal(0.3333, result, 3);
    }

    [Fact]
    public void AmericanToImpliedProbability_Negative()
    {
        // -200 => decimal 1.5 => implied 66.67%
        var result = OddsCalculator.AmericanToImpliedProbability(-200);
        Assert.Equal(0.6667, result, 3);
    }
}
