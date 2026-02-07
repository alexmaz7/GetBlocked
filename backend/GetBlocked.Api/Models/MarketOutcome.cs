namespace GetBlocked.Api.Models;

public class MarketOutcome
{
    public string Name { get; set; } = string.Empty;
    public List<BookOdds> Books { get; set; } = new();
    public BookOdds BestOdds { get; set; } = new();
    public double ConsensusProbability { get; set; }
}
