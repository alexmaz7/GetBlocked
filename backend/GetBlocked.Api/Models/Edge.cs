namespace GetBlocked.Api.Models;

public enum EdgeType
{
    Value,
    Arbitrage,
    LineMovement
}

public enum EdgeRating
{
    Low,
    Medium,
    High
}

public class ValueBet
{
    public string Type => "value";
    public string EventId { get; set; } = string.Empty;
    public string Matchup { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public DateTime CommenceTime { get; set; }
    public string Market { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public string Sportsbook { get; set; } = string.Empty;
    public int AmericanOdds { get; set; }
    public double DecimalOdds { get; set; }
    public double ImpliedProbability { get; set; }
    public double EstimatedTrueProbability { get; set; }
    public double Edge { get; set; }
    public double KellyStake { get; set; }
    public string Rating { get; set; } = string.Empty;
}

public class ArbitrageBet
{
    public string Type => "arbitrage";
    public string EventId { get; set; } = string.Empty;
    public string Matchup { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public DateTime CommenceTime { get; set; }
    public string Market { get; set; } = string.Empty;
    public List<ArbitrageLeg> Legs { get; set; } = new();
    public double TotalImplied { get; set; }
    public double ProfitMargin { get; set; }
    public string Rating { get; set; } = string.Empty;
}

public class ArbitrageLeg
{
    public string Outcome { get; set; } = string.Empty;
    public string Sportsbook { get; set; } = string.Empty;
    public int AmericanOdds { get; set; }
    public double DecimalOdds { get; set; }
    public double StakePercent { get; set; }
}

public class LineMovement
{
    public string Type => "line_movement";
    public string EventId { get; set; } = string.Empty;
    public string Matchup { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public DateTime CommenceTime { get; set; }
    public string Market { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public int CurrentBestOdds { get; set; }
    public string Direction { get; set; } = string.Empty;
    public double Magnitude { get; set; }
    public string Rating { get; set; } = string.Empty;
}

public class EdgeResult
{
    public List<ValueBet> ValueBets { get; set; } = new();
    public List<ArbitrageBet> ArbitrageBets { get; set; } = new();
    public List<LineMovement> LineMovements { get; set; } = new();
    public EdgeSummary Summary { get; set; } = new();
}

public class EdgeSummary
{
    public int TotalEdges { get; set; }
    public int ValueBetCount { get; set; }
    public int ArbitrageCount { get; set; }
    public int LineMovementCount { get; set; }
    public int EventsScanned { get; set; }
    public double TotalKellyExposure { get; set; }
    public double Bankroll { get; set; }
}
