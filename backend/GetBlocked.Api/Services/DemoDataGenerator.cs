using GetBlocked.Api.Models;

namespace GetBlocked.Api.Services;

/// <summary>
/// Generates realistic simulated odds data for demo mode.
/// Includes intentional edges so the finder has something to detect.
/// </summary>
public static class DemoDataGenerator
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

    private static Market MakeMarket(string key, string label, params MarketOutcome[] outcomes)
    {
        var list = outcomes.ToList();
        var totalImplied = list.Sum(o => o.ConsensusProbability);

        return new Market
        {
            Key = key,
            Label = label,
            Outcomes = list,
            TotalImpliedProbability = Math.Round(totalImplied, 4),
            Overround = Math.Round(totalImplied - 1, 4),
        };
    }

    public static List<SportEvent> Generate()
    {
        var now = DateTime.UtcNow;

        return new List<SportEvent>
        {
            // NFL: value bet — Caesars has KC at -130 while consensus is ~-150
            new()
            {
                Id = "demo-nfl-1",
                Sport = "americanfootball_nfl",
                League = "NFL",
                HomeTeam = "Kansas City Chiefs",
                AwayTeam = "Buffalo Bills",
                CommenceTime = now.AddHours(3),
                Markets = new List<Market>
                {
                    MakeMarket("h2h", "Moneyline",
                        MakeOutcome("Kansas City Chiefs",
                            MakeBook("DraftKings", -155),
                            MakeBook("FanDuel", -150),
                            MakeBook("BetMGM", -148),
                            MakeBook("Caesars", -130),
                            MakeBook("PointsBet", -152)),
                        MakeOutcome("Buffalo Bills",
                            MakeBook("DraftKings", 135),
                            MakeBook("FanDuel", 130),
                            MakeBook("BetMGM", 128),
                            MakeBook("Caesars", 110),
                            MakeBook("PointsBet", 132))
                    ),
                    MakeMarket("spreads", "Spread",
                        MakeOutcome("Kansas City Chiefs -3.5",
                            MakeBook("DraftKings", -110),
                            MakeBook("FanDuel", -108),
                            MakeBook("BetMGM", -112),
                            MakeBook("Caesars", -105),
                            MakeBook("PointsBet", -110)),
                        MakeOutcome("Buffalo Bills +3.5",
                            MakeBook("DraftKings", -110),
                            MakeBook("FanDuel", -112),
                            MakeBook("BetMGM", -108),
                            MakeBook("Caesars", -115),
                            MakeBook("PointsBet", -110))
                    ),
                },
            },

            // NBA: arbitrage opportunity
            new()
            {
                Id = "demo-nba-1",
                Sport = "basketball_nba",
                League = "NBA",
                HomeTeam = "Boston Celtics",
                AwayTeam = "Milwaukee Bucks",
                CommenceTime = now.AddHours(5),
                Markets = new List<Market>
                {
                    MakeMarket("h2h", "Moneyline",
                        MakeOutcome("Boston Celtics",
                            MakeBook("DraftKings", -135),
                            MakeBook("FanDuel", -140),
                            MakeBook("BetMGM", -125),
                            MakeBook("Caesars", -138),
                            MakeBook("PointsBet", -142)),
                        MakeOutcome("Milwaukee Bucks",
                            MakeBook("DraftKings", 115),
                            MakeBook("FanDuel", 120),
                            MakeBook("BetMGM", 105),
                            MakeBook("Caesars", 118),
                            MakeBook("PointsBet", 130))
                    ),
                    MakeMarket("totals", "Totals",
                        MakeOutcome("Over 224.5",
                            MakeBook("DraftKings", -108),
                            MakeBook("FanDuel", -110),
                            MakeBook("BetMGM", -105),
                            MakeBook("Caesars", -110),
                            MakeBook("PointsBet", -115)),
                        MakeOutcome("Under 224.5",
                            MakeBook("DraftKings", -112),
                            MakeBook("FanDuel", -110),
                            MakeBook("BetMGM", -115),
                            MakeBook("Caesars", -110),
                            MakeBook("PointsBet", -105))
                    ),
                },
            },

            // EPL: line movement signal
            new()
            {
                Id = "demo-epl-1",
                Sport = "soccer_epl",
                League = "English Premier League",
                HomeTeam = "Arsenal",
                AwayTeam = "Manchester City",
                CommenceTime = now.AddHours(24),
                Markets = new List<Market>
                {
                    MakeMarket("h2h", "Moneyline",
                        MakeOutcome("Arsenal",
                            MakeBook("DraftKings", 165),
                            MakeBook("FanDuel", 160),
                            MakeBook("BetMGM", 170),
                            MakeBook("Bet365", 185),
                            MakeBook("William Hill", 155)),
                        MakeOutcome("Manchester City",
                            MakeBook("DraftKings", 150),
                            MakeBook("FanDuel", 155),
                            MakeBook("BetMGM", 145),
                            MakeBook("Bet365", 130),
                            MakeBook("William Hill", 160)),
                        MakeOutcome("Draw",
                            MakeBook("DraftKings", 240),
                            MakeBook("FanDuel", 235),
                            MakeBook("BetMGM", 245),
                            MakeBook("Bet365", 250),
                            MakeBook("William Hill", 230))
                    ),
                },
            },

            // NHL: value on moneyline
            new()
            {
                Id = "demo-nhl-1",
                Sport = "icehockey_nhl",
                League = "NHL",
                HomeTeam = "Toronto Maple Leafs",
                AwayTeam = "Montreal Canadiens",
                CommenceTime = now.AddHours(7),
                Markets = new List<Market>
                {
                    MakeMarket("h2h", "Moneyline",
                        MakeOutcome("Toronto Maple Leafs",
                            MakeBook("DraftKings", -180),
                            MakeBook("FanDuel", -175),
                            MakeBook("BetMGM", -185),
                            MakeBook("Caesars", -160),
                            MakeBook("PointsBet", -178)),
                        MakeOutcome("Montreal Canadiens",
                            MakeBook("DraftKings", 155),
                            MakeBook("FanDuel", 150),
                            MakeBook("BetMGM", 160),
                            MakeBook("Caesars", 140),
                            MakeBook("PointsBet", 153))
                    ),
                    MakeMarket("totals", "Totals",
                        MakeOutcome("Over 6.5",
                            MakeBook("DraftKings", 105),
                            MakeBook("FanDuel", -102),
                            MakeBook("BetMGM", 110),
                            MakeBook("Caesars", -105),
                            MakeBook("PointsBet", 108)),
                        MakeOutcome("Under 6.5",
                            MakeBook("DraftKings", -125),
                            MakeBook("FanDuel", -118),
                            MakeBook("BetMGM", -130),
                            MakeBook("Caesars", -115),
                            MakeBook("PointsBet", -128))
                    ),
                },
            },
        };
    }
}
