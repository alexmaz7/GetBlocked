namespace GetBlocked.Api.Models;

public class EdgeFinderConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public List<string> Sports { get; set; } = new() { "americanfootball_nfl", "basketball_nba" };
    public double MinEdge { get; set; } = 0.02;
    public double Bankroll { get; set; } = 1000;
    public double KellyFraction { get; set; } = 0.25;
    public string Regions { get; set; } = "us";
    public string Markets { get; set; } = "h2h,spreads,totals";
}
