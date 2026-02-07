using GetBlocked.Api.Models;
using GetBlocked.Api.Services;
using Xunit;

namespace GetBlocked.Api.Tests;

public class EdgeDetectorTests
{
    private static BookOdds MakeBook(string name, int american)
    {
        var dec = OddsCalculator.AmericanToDecimal(american);
        return new BookOdds
        {
            Sportsbook = name,
            AmericanOdds = american,
            DecimalOdds = Math.Round(dec, 3),
            ImpliedProbability = Math.Round(OddsCalculator.ImpliedProbability(dec), 4),
        };
    }

    private static MarketOutcome MakeOutcome(string name, params BookOdds[] books)
    {
        var bookList = books.ToList();
        var best = bookList.OrderByDescending(b => b.DecimalOdds).First();
        var consensus = bookList.Average(b => b.ImpliedProbability);
        return new MarketOutcome
        {
            Name = name,
            Books = bookList,
            BestOdds = best,
            ConsensusProbability = Math.Round(consensus, 4),
        };
    }

    private static SportEvent CreateEventWithMoneyline(
        string name1, int[] odds1,
        string name2, int[] odds2)
    {
        var books = new[] { "BookA", "BookB", "BookC", "BookD", "BookE" };
        var outcome1Books = odds1.Select((o, i) => MakeBook(books[i], o)).ToArray();
        var outcome2Books = odds2.Select((o, i) => MakeBook(books[i], o)).ToArray();

        var outcome1 = MakeOutcome(name1, outcome1Books);
        var outcome2 = MakeOutcome(name2, outcome2Books);

        var totalImplied = outcome1.ConsensusProbability + outcome2.ConsensusProbability;
        var market = new Market
        {
            Key = "h2h",
            Label = "Moneyline",
            Outcomes = new List<MarketOutcome> { outcome1, outcome2 },
            TotalImpliedProbability = Math.Round(totalImplied, 4),
            Overround = Math.Round(totalImplied - 1, 4),
        };

        return new SportEvent
        {
            Id = "test-1",
            Sport = "test",
            League = "Test League",
            HomeTeam = name1,
            AwayTeam = name2,
            CommenceTime = DateTime.UtcNow.AddHours(2),
            Markets = new List<Market> { market },
        };
    }

    [Fact]
    public void FindValueBets_DetectsOutlierOdds()
    {
        // Most books have Team A around -150, but one has -130 (value)
        var evt = CreateEventWithMoneyline(
            "Team A", new[] { -155, -150, -148, -130, -152 },
            "Team B", new[] { 135, 130, 128, 110, 132 }
        );

        var detector = new EdgeDetector(minEdge: 0.01, bankroll: 1000, kellyFraction: 0.25);
        var valueBets = detector.FindValueBets(evt);

        Assert.NotEmpty(valueBets);

        // The -130 outlier should be detected as a value bet on Team A
        var teamAValueBet = valueBets.FirstOrDefault(v => v.Outcome == "Team A" && v.Sportsbook == "BookD");
        Assert.NotNull(teamAValueBet);
        Assert.True(teamAValueBet.Edge > 0);
        Assert.True(teamAValueBet.KellyStake > 0);
    }

    [Fact]
    public void FindValueBets_NoEdge_ReturnsEmpty()
    {
        // All books have same odds — no value
        var evt = CreateEventWithMoneyline(
            "Team A", new[] { -110, -110, -110, -110, -110 },
            "Team B", new[] { -110, -110, -110, -110, -110 }
        );

        var detector = new EdgeDetector(minEdge: 0.02);
        var valueBets = detector.FindValueBets(evt);

        Assert.Empty(valueBets);
    }

    [Fact]
    public void FindArbitrage_DetectsArbitrageOpportunity()
    {
        // Best odds on each side: BetMGM -125 (implied 55.56%) and PointsBet +130 (implied 43.48%)
        // Total: 99.04% < 100% = arbitrage!
        var evt = CreateEventWithMoneyline(
            "Team A", new[] { -135, -140, -125, -138, -142 },
            "Team B", new[] { 115, 120, 105, 118, 130 }
        );

        var detector = new EdgeDetector();
        var arbs = detector.FindArbitrage(evt);

        Assert.NotEmpty(arbs);
        Assert.True(arbs[0].ProfitMargin > 0);
        Assert.True(arbs[0].TotalImplied < 1);
        Assert.Equal(2, arbs[0].Legs.Count);
    }

    [Fact]
    public void FindArbitrage_NoArbitrage_ReturnsEmpty()
    {
        // Standard vig — no arb
        var evt = CreateEventWithMoneyline(
            "Team A", new[] { -110, -110, -110, -110, -110 },
            "Team B", new[] { -110, -110, -110, -110, -110 }
        );

        var detector = new EdgeDetector();
        var arbs = detector.FindArbitrage(evt);

        Assert.Empty(arbs);
    }

    [Fact]
    public void FindLineMovements_DetectsSpread()
    {
        // Wide spread in odds across books
        var evt = CreateEventWithMoneyline(
            "Team A", new[] { -180, -175, -185, -160, -178 },
            "Team B", new[] { 155, 150, 160, 140, 153 }
        );

        var detector = new EdgeDetector();
        var movements = detector.FindLineMovements(evt, minMagnitude: 0.02);

        Assert.NotEmpty(movements);
    }

    [Fact]
    public void FindAllEdges_CombinesAllTypes()
    {
        var events = DemoDataGenerator.Generate();
        var detector = new EdgeDetector(minEdge: 0.01);
        var result = detector.FindAllEdges(events);

        Assert.True(result.Summary.TotalEdges > 0);
        Assert.Equal(events.Count, result.Summary.EventsScanned);
        Assert.Equal(
            result.ValueBets.Count + result.ArbitrageBets.Count + result.LineMovements.Count,
            result.Summary.TotalEdges);
    }

    [Fact]
    public void FindAllEdges_DemoData_FindsValueBets()
    {
        var events = DemoDataGenerator.Generate();
        var detector = new EdgeDetector(minEdge: 0.01);
        var result = detector.FindAllEdges(events);

        Assert.True(result.ValueBets.Count > 0);
    }

    [Fact]
    public void ValueBet_HasValidRating()
    {
        var events = DemoDataGenerator.Generate();
        var detector = new EdgeDetector(minEdge: 0.01);
        var result = detector.FindAllEdges(events);

        foreach (var vb in result.ValueBets)
        {
            Assert.Contains(vb.Rating, new[] { "low", "medium", "high" });
        }
    }

    [Fact]
    public void ArbitrageBet_LegsStakePercent_SumToOne()
    {
        var events = DemoDataGenerator.Generate();
        var detector = new EdgeDetector(minEdge: 0.01);
        var result = detector.FindAllEdges(events);

        foreach (var arb in result.ArbitrageBets)
        {
            var totalStake = arb.Legs.Sum(l => l.StakePercent);
            Assert.Equal(1.0, totalStake, 2);
        }
    }
}
