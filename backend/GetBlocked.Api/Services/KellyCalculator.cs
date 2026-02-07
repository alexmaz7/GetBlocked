namespace GetBlocked.Api.Services;

/// <summary>
/// Kelly Criterion bankroll management.
/// Determines the optimal fraction of bankroll to wager given edge and odds.
/// </summary>
public static class KellyCalculator
{
    public record KellyResult(
        double FullKelly,
        double FractionalKelly,
        double RecommendedStake,
        double ExpectedValue
    );

    /// <summary>
    /// Calculate the Kelly stake for a single bet.
    /// </summary>
    /// <param name="trueProbability">Estimated true probability of winning (0–1)</param>
    /// <param name="decimalOdds">Decimal odds offered by the sportsbook</param>
    /// <param name="bankroll">Total bankroll in dollars</param>
    /// <param name="fraction">Kelly fraction (e.g. 0.25 = quarter Kelly)</param>
    public static KellyResult Calculate(
        double trueProbability,
        double decimalOdds,
        double bankroll,
        double fraction = 0.25)
    {
        var q = 1 - trueProbability;
        var b = decimalOdds - 1; // net odds

        // Kelly formula: f* = (bp - q) / b
        var fullKelly = (b * trueProbability - q) / b;

        // Clamp between 0 and 1
        var clampedFull = Math.Max(0, Math.Min(1, fullKelly));
        var fractionalKelly = clampedFull * fraction;
        var recommendedStake = Math.Round(fractionalKelly * bankroll, 2);
        var expectedValue = Math.Round(trueProbability * b - q, 4);

        return new KellyResult(
            Math.Round(clampedFull, 4),
            Math.Round(fractionalKelly, 4),
            recommendedStake,
            expectedValue
        );
    }
}
