using Microsoft.AspNetCore.Mvc;
using GetBlocked.Api.Services;

namespace GetBlocked.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OddsController : ControllerBase
{
    /// <summary>
    /// Convert between odds formats.
    /// </summary>
    [HttpGet("convert")]
    public ActionResult Convert(
        [FromQuery] int? american,
        [FromQuery] double? decimalOdds)
    {
        if (american.HasValue)
        {
            var dec = OddsCalculator.AmericanToDecimal(american.Value);
            return Ok(new
            {
                american = american.Value,
                decimalOdds = Math.Round(dec, 3),
                impliedProbability = Math.Round(OddsCalculator.ImpliedProbability(dec), 4),
            });
        }

        if (decimalOdds.HasValue)
        {
            var am = OddsCalculator.DecimalToAmerican(decimalOdds.Value);
            return Ok(new
            {
                american = am,
                decimalOdds = decimalOdds.Value,
                impliedProbability = Math.Round(OddsCalculator.ImpliedProbability(decimalOdds.Value), 4),
            });
        }

        return BadRequest("Provide either 'american' or 'decimalOdds' query parameter.");
    }

    /// <summary>
    /// Calculate Kelly Criterion stake.
    /// </summary>
    [HttpGet("kelly")]
    public ActionResult Kelly(
        [FromQuery] double trueProbability,
        [FromQuery] double decimalOdds,
        [FromQuery] double bankroll = 1000,
        [FromQuery] double fraction = 0.25)
    {
        if (trueProbability <= 0 || trueProbability >= 1)
            return BadRequest("trueProbability must be between 0 and 1 (exclusive).");
        if (decimalOdds <= 1)
            return BadRequest("decimalOdds must be greater than 1.");

        var result = KellyCalculator.Calculate(trueProbability, decimalOdds, bankroll, fraction);
        return Ok(result);
    }
}
