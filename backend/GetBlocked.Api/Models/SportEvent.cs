namespace GetBlocked.Api.Models;

public class SportEvent
{
    public string Id { get; set; } = string.Empty;
    public string Sport { get; set; } = string.Empty;
    public string League { get; set; } = string.Empty;
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public DateTime CommenceTime { get; set; }
    public List<Market> Markets { get; set; } = new();
}
