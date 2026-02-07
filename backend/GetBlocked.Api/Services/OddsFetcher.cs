using System.Net.Http.Json;
using GetBlocked.Api.Models;

namespace GetBlocked.Api.Services;

/// <summary>
/// Fetches live odds from The Odds API and transforms them into our domain model.
/// </summary>
public class OddsFetcher
{
    private const string BaseUrl = "https://api.the-odds-api.com/v4";
    private readonly HttpClient _http;

    private static readonly Dictionary<string, string> MarketLabels = new()
    {
        ["h2h"] = "Moneyline",
        ["spreads"] = "Spread",
        ["totals"] = "Totals",
    };

    public OddsFetcher(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SportEvent>> FetchOddsAsync(EdgeFinderConfig config)
    {
        var events = new List<SportEvent>();
        var marketKeys = config.Markets.Split(',');

        foreach (var sport in config.Sports)
        {
            try
            {
                var url = $"{BaseUrl}/sports/{sport}/odds?apiKey={config.ApiKey}" +
                          $"&regions={config.Regions}&markets={config.Markets}&oddsFormat=american";

                var apiEvents = await _http.GetFromJsonAsync<List<OddsApiEvent>>(url);
                if (apiEvents == null) continue;

                foreach (var apiEvent in apiEvents)
                {
                    events.Add(TransformEvent(apiEvent, marketKeys));
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new InvalidOperationException("Invalid API key. Get a free key at https://the-odds-api.com");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                // Rate limited — skip this sport
                continue;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to fetch odds for {sport}: {ex.Message}");
            }
        }

        return events;
    }

    private static SportEvent TransformEvent(OddsApiEvent apiEvent, string[] marketKeys)
    {
        var markets = new List<Market>();

        foreach (var key in marketKeys)
        {
            var market = TransformMarket(key, apiEvent.Bookmakers);
            if (market != null)
                markets.Add(market);
        }

        return new SportEvent
        {
            Id = apiEvent.Id,
            Sport = apiEvent.SportKey,
            League = apiEvent.SportTitle,
            HomeTeam = apiEvent.HomeTeam,
            AwayTeam = apiEvent.AwayTeam,
            CommenceTime = apiEvent.CommenceTime,
            Markets = markets,
        };
    }

    private static Market? TransformMarket(string marketKey, List<OddsApiBookmaker> bookmakers)
    {
        var outcomeMap = new Dictionary<string, List<BookOdds>>();

        foreach (var bookmaker in bookmakers)
        {
            var apiMarket = bookmaker.Markets.FirstOrDefault(m => m.Key == marketKey);
            if (apiMarket == null) continue;

            foreach (var outcome in apiMarket.Outcomes)
            {
                var label = outcome.Point.HasValue
                    ? $"{outcome.Name} {outcome.Point}"
                    : outcome.Name;

                if (!outcomeMap.ContainsKey(label))
                    outcomeMap[label] = new List<BookOdds>();

                var decimalOdds = OddsCalculator.AmericanToDecimal(outcome.Price);
                outcomeMap[label].Add(new BookOdds
                {
                    Sportsbook = bookmaker.Title,
                    AmericanOdds = outcome.Price,
                    DecimalOdds = Math.Round(decimalOdds, 3),
                    ImpliedProbability = Math.Round(OddsCalculator.ImpliedProbability(decimalOdds), 4),
                });
            }
        }

        if (outcomeMap.Count == 0) return null;

        var outcomes = new List<MarketOutcome>();
        foreach (var (name, books) in outcomeMap)
        {
            var bestOdds = books.OrderByDescending(b => b.DecimalOdds).First();
            var consensus = books.Average(b => b.ImpliedProbability);

            outcomes.Add(new MarketOutcome
            {
                Name = name,
                Books = books,
                BestOdds = bestOdds,
                ConsensusProbability = Math.Round(consensus, 4),
            });
        }

        var totalImplied = outcomes.Sum(o => o.ConsensusProbability);

        return new Market
        {
            Key = marketKey,
            Label = MarketLabels.GetValueOrDefault(marketKey, marketKey),
            Outcomes = outcomes,
            TotalImpliedProbability = Math.Round(totalImplied, 4),
            Overround = Math.Round(totalImplied - 1, 4),
        };
    }
}
