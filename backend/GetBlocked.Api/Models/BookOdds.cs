namespace GetBlocked.Api.Models;

public class BookOdds
{
    public string Sportsbook { get; set; } = string.Empty;
    public int AmericanOdds { get; set; }
    public double DecimalOdds { get; set; }
    public double ImpliedProbability { get; set; }
}
