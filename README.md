# GetBlocked — Sports Betting Edge Finder

Scans sportsbook odds across multiple books to detect **value bets**, **arbitrage opportunities**, and **line movement signals**.

## Architecture

- **Backend**: C# / ASP.NET Core 8 Web API
- **Frontend**: React + TypeScript

## Features

- **Value Bets** — Finds outcomes where a sportsbook's price is softer than the market consensus (de-vigged via power method)
- **Arbitrage Detection** — Identifies guaranteed-profit opportunities when best odds across books sum to <100% implied probability
- **Line Movement Signals** — Flags steam moves and reverse line movement across books
- **Kelly Criterion** — Calculates optimal stake sizing with configurable fractional Kelly
- **Multi-sport** — NFL, NBA, MLB, NHL, NCAAF, NCAAB, EPL, MLS, MMA, Champions League
- **The Odds API Integration** — Live odds from 10+ US sportsbooks
- **Demo Mode** — Try with simulated data (no API key required)

## Getting Started

### Backend (C# API)

```bash
cd backend/GetBlocked.Api
dotnet restore
dotnet run
# API available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

### Frontend (React)

```bash
cd frontend
npm install
npm start
# Opens at http://localhost:3000
```

### Run Tests

```bash
cd backend/GetBlocked.Api.Tests
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/edges/demo` | Scan demo data for edges |
| `POST` | `/api/edges/scan` | Scan live odds (requires API key) |
| `GET` | `/api/odds/convert` | Convert between odds formats |
| `GET` | `/api/odds/kelly` | Calculate Kelly Criterion stake |

### Demo scan

```
GET /api/edges/demo?minEdge=0.02&bankroll=1000&kellyFraction=0.25
```

### Live scan

```
POST /api/edges/scan
{
  "apiKey": "your-key-from-the-odds-api.com",
  "sports": ["americanfootball_nfl", "basketball_nba"],
  "minEdge": 0.02,
  "bankroll": 1000,
  "kellyFraction": 0.25,
  "regions": "us",
  "markets": "h2h,spreads,totals"
}
```

## How It Works

1. **Fetch** odds from multiple sportsbooks via The Odds API
2. **Build consensus** — average implied probabilities across all books for each outcome
3. **De-vig** — remove the bookmaker's margin using power method (2-way) or normalization (3-way)
4. **Compare** each book's price against the consensus fair probability
5. **Flag edges** — if a book's implied probability is lower than the true probability by more than `minEdge`, it's a value bet
6. **Size bets** — Kelly Criterion determines optimal stake based on edge magnitude
7. **Check arbitrage** — if best odds across books sum to <100%, guaranteed profit exists

## License

MIT
