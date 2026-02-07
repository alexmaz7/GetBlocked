using Microsoft.AspNetCore.Mvc;
using GetBlocked.Api.Models;
using GetBlocked.Api.Services;

namespace GetBlocked.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EdgesController : ControllerBase
{
    private readonly OddsFetcher _fetcher;

    public EdgesController(OddsFetcher fetcher)
    {
        _fetcher = fetcher;
    }

    /// <summary>
    /// Scan for edges using live odds from The Odds API.
    /// </summary>
    [HttpPost("scan")]
    public async Task<ActionResult<EdgeResult>> Scan([FromBody] EdgeFinderConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.ApiKey))
            return BadRequest("API key is required. Get a free key at https://the-odds-api.com");

        var events = await _fetcher.FetchOddsAsync(config);
        var detector = new EdgeDetector(config.MinEdge, config.Bankroll, config.KellyFraction);
        var result = detector.FindAllEdges(events);

        return Ok(result);
    }

    /// <summary>
    /// Scan using demo data — no API key required.
    /// </summary>
    [HttpGet("demo")]
    public ActionResult<EdgeResult> Demo(
        [FromQuery] double minEdge = 0.02,
        [FromQuery] double bankroll = 1000,
        [FromQuery] double kellyFraction = 0.25)
    {
        var events = DemoDataGenerator.Generate();
        var detector = new EdgeDetector(minEdge, bankroll, kellyFraction);
        var result = detector.FindAllEdges(events);

        return Ok(result);
    }
}
