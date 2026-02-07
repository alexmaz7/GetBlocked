namespace GetBlocked.Api.Services;

/// <summary>
/// Odds conversion and implied probability calculations.
/// </summary>
public static class OddsCalculator
{
    /// <summary>Convert American odds to decimal odds.</summary>
    public static double AmericanToDecimal(int american)
    {
        if (american > 0)
            return american / 100.0 + 1;
        return 100.0 / Math.Abs(american) + 1;
    }

    /// <summary>Convert decimal odds to American odds.</summary>
    public static int DecimalToAmerican(double decimalOdds)
    {
        if (decimalOdds >= 2)
            return (int)Math.Round((decimalOdds - 1) * 100);
        return (int)Math.Round(-100 / (decimalOdds - 1));
    }

    /// <summary>Get implied probability from decimal odds.</summary>
    public static double ImpliedProbability(double decimalOdds)
    {
        if (decimalOdds <= 1) return 1;
        return 1.0 / decimalOdds;
    }

    /// <summary>Get implied probability from American odds.</summary>
    public static double AmericanToImpliedProbability(int american)
    {
        return ImpliedProbability(AmericanToDecimal(american));
    }

    /// <summary>
    /// Remove the vig by normalizing implied probabilities to sum to 1.
    /// </summary>
    public static double[] RemoveVig(double[] impliedProbs)
    {
        var total = impliedProbs.Sum();
        if (total == 0) return impliedProbs;
        return impliedProbs.Select(p => p / total).ToArray();
    }

    /// <summary>Calculate overround (vig) for a set of implied probabilities.</summary>
    public static double CalculateOverround(double[] impliedProbs)
    {
        return impliedProbs.Sum();
    }

    /// <summary>
    /// Power method vig removal — more accurate for two-outcome markets.
    /// Solves for exponent k such that p1^k + p2^k = 1.
    /// </summary>
    public static double[] PowerDevig(double[] impliedProbs)
    {
        if (impliedProbs.Length != 2) return RemoveVig(impliedProbs);

        var p1 = impliedProbs[0];
        var p2 = impliedProbs[1];

        // Binary search for k
        double lo = 0.01, hi = 5;
        for (int i = 0; i < 100; i++)
        {
            var mid = (lo + hi) / 2;
            var sum = Math.Pow(p1, mid) + Math.Pow(p2, mid);
            if (sum > 1)
                lo = mid;
            else
                hi = mid;
        }

        var k = (lo + hi) / 2;
        return new[] { Math.Pow(p1, k), Math.Pow(p2, k) };
    }
}
