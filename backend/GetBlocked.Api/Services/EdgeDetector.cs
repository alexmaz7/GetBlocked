using GetBlocked.Api.Models;

namespace GetBlocked.Api.Services;

/// <summary>
/// Core edge detection algorithms.
/// Finds value bets, arbitrage opportunities, and line movement signals.
/// </summary>
public class EdgeDetector
{
    private readonly double _minEdge;
    private readonly double _bankroll;
    private readonly double _kellyFraction;

    public EdgeDetector(double minEdge = 0.02, double bankroll = 1000, double kellyFraction = 0.25)
    {
        _minEdge = minEdge;
        _bankroll = bankroll;
        _kellyFraction = kellyFraction;
    }

    private static string RateEdge(double edge) =>
        edge >= 0.08 ? "high" : edge >= 0.04 ? "medium" : "low";

    /// <summary>
    /// Find value bets — outcomes where a book's price exceeds the consensus fair probability.
    /// </summary>
    public List<ValueBet> FindValueBets(SportEvent evt)
    {
        var valueBets = new List<ValueBet>();
        var matchup = $"{evt.AwayTeam} @ {evt.HomeTeam}";

        foreach (var market in evt.Markets)
        {
            // Build consensus: average implied prob across all books, then de-vig
            var avgProbs = market.Outcomes
                .Select(o => o.Books.Count == 0 ? 0 : o.Books.Average(b => b.ImpliedProbability))
                .ToArray();

            var devigged = market.Outcomes.Count == 2
                ? OddsCalculator.PowerDevig(avgProbs)
                : OddsCalculator.RemoveVig(avgProbs);

            for (int i = 0; i < market.Outcomes.Count; i++)
            {
                var outcome = market.Outcomes[i];
                var trueProbability = devigged[i];

                foreach (var book in outcome.Books)
                {
                    var edge = trueProbability - book.ImpliedProbability;

                    if (edge >= _minEdge)
                    {
                        var kelly = KellyCalculator.Calculate(
                            trueProbability, book.DecimalOdds, _bankroll, _kellyFraction);

                        valueBets.Add(new ValueBet
                        {
                            EventId = evt.Id,
                            Matchup = matchup,
                            League = evt.League,
                            CommenceTime = evt.CommenceTime,
                            Market = market.Label,
                            Outcome = outcome.Name,
                            Sportsbook = book.Sportsbook,
                            AmericanOdds = book.AmericanOdds,
                            DecimalOdds = book.DecimalOdds,
                            ImpliedProbability = Math.Round(book.ImpliedProbability, 4),
                            EstimatedTrueProbability = Math.Round(trueProbability, 4),
                            Edge = Math.Round(edge, 4),
                            KellyStake = kelly.RecommendedStake,
                            Rating = RateEdge(edge),
                        });
                    }
                }
            }
        }

        return valueBets.OrderByDescending(v => v.Edge).ToList();
    }

    /// <summary>
    /// Find arbitrage — where best odds across books sum to less than 100% implied probability.
    /// </summary>
    public List<ArbitrageBet> FindArbitrage(SportEvent evt)
    {
        var arbs = new List<ArbitrageBet>();
        var matchup = $"{evt.AwayTeam} @ {evt.HomeTeam}";

        foreach (var market in evt.Markets)
        {
            if (market.Outcomes.Count < 2) continue;

            var bestBooks = market.Outcomes.Select(o => o.BestOdds).ToList();
            var totalImplied = bestBooks.Sum(b => b.ImpliedProbability);

            if (totalImplied < 1)
            {
                var profitMargin = 1 - totalImplied;

                var legs = market.Outcomes.Select((o, i) => new ArbitrageLeg
                {
                    Outcome = o.Name,
                    Sportsbook = bestBooks[i].Sportsbook,
                    AmericanOdds = bestBooks[i].AmericanOdds,
                    DecimalOdds = bestBooks[i].DecimalOdds,
                    StakePercent = Math.Round(bestBooks[i].ImpliedProbability / totalImplied, 4),
                }).ToList();

                arbs.Add(new ArbitrageBet
                {
                    EventId = evt.Id,
                    Matchup = matchup,
                    League = evt.League,
                    CommenceTime = evt.CommenceTime,
                    Market = market.Label,
                    Legs = legs,
                    TotalImplied = Math.Round(totalImplied, 4),
                    ProfitMargin = Math.Round(profitMargin, 4),
                    Rating = RateEdge(profitMargin),
                });
            }
        }

        return arbs.OrderByDescending(a => a.ProfitMargin).ToList();
    }

    /// <summary>
    /// Detect line movement signals via odds spread across books.
    /// </summary>
    public List<LineMovement> FindLineMovements(SportEvent evt, double minMagnitude = 0.03)
    {
        var movements = new List<LineMovement>();
        var matchup = $"{evt.AwayTeam} @ {evt.HomeTeam}";

        foreach (var market in evt.Markets)
        {
            foreach (var outcome in market.Outcomes)
            {
                if (outcome.Books.Count < 3) continue;

                var probs = outcome.Books.Select(b => b.ImpliedProbability).ToList();
                var avgProb = probs.Average();
                var bestProb = outcome.BestOdds.ImpliedProbability;
                var worstProb = probs.Max();
                var spread = worstProb - bestProb;

                if (spread >= minMagnitude)
                {
                    var direction = bestProb < avgProb - minMagnitude / 2 ? "steam" : "reverse";

                    movements.Add(new LineMovement
                    {
                        EventId = evt.Id,
                        Matchup = matchup,
                        League = evt.League,
                        CommenceTime = evt.CommenceTime,
                        Market = market.Label,
                        Outcome = outcome.Name,
                        CurrentBestOdds = outcome.BestOdds.AmericanOdds,
                        Direction = direction,
                        Magnitude = Math.Round(spread, 4),
                        Rating = RateEdge(spread),
                    });
                }
            }
        }

        return movements.OrderByDescending(m => m.Magnitude).ToList();
    }

    /// <summary>Run all edge detection on an event.</summary>
    public EdgeResult FindAllEdges(List<SportEvent> events)
    {
        var result = new EdgeResult();

        foreach (var evt in events)
        {
            result.ValueBets.AddRange(FindValueBets(evt));
            result.ArbitrageBets.AddRange(FindArbitrage(evt));
            result.LineMovements.AddRange(FindLineMovements(evt));
        }

        result.Summary = new EdgeSummary
        {
            TotalEdges = result.ValueBets.Count + result.ArbitrageBets.Count + result.LineMovements.Count,
            ValueBetCount = result.ValueBets.Count,
            ArbitrageCount = result.ArbitrageBets.Count,
            LineMovementCount = result.LineMovements.Count,
            EventsScanned = events.Count,
            TotalKellyExposure = Math.Round(result.ValueBets.Sum(v => v.KellyStake), 2),
            Bankroll = _bankroll,
        };

        return result;
    }
}
