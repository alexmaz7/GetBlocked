namespace GetBlocked.Api.Models;

public class Market
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public List<MarketOutcome> Outcomes { get; set; } = new();
    public double TotalImpliedProbability { get; set; }
    public double Overround { get; set; }
}
