export interface ValueBet {
  type: 'value';
  eventId: string;
  matchup: string;
  league: string;
  commenceTime: string;
  market: string;
  outcome: string;
  sportsbook: string;
  americanOdds: number;
  decimalOdds: number;
  impliedProbability: number;
  estimatedTrueProbability: number;
  edge: number;
  kellyStake: number;
  rating: 'low' | 'medium' | 'high';
}

export interface ArbitrageLeg {
  outcome: string;
  sportsbook: string;
  americanOdds: number;
  decimalOdds: number;
  stakePercent: number;
}

export interface ArbitrageBet {
  type: 'arbitrage';
  eventId: string;
  matchup: string;
  league: string;
  commenceTime: string;
  market: string;
  legs: ArbitrageLeg[];
  totalImplied: number;
  profitMargin: number;
  rating: 'low' | 'medium' | 'high';
}

export interface LineMovement {
  type: 'line_movement';
  eventId: string;
  matchup: string;
  league: string;
  commenceTime: string;
  market: string;
  outcome: string;
  currentBestOdds: number;
  direction: 'steam' | 'reverse';
  magnitude: number;
  rating: 'low' | 'medium' | 'high';
}

export interface EdgeSummary {
  totalEdges: number;
  valueBetCount: number;
  arbitrageCount: number;
  lineMovementCount: number;
  eventsScanned: number;
  totalKellyExposure: number;
  bankroll: number;
}

export interface EdgeResult {
  valueBets: ValueBet[];
  arbitrageBets: ArbitrageBet[];
  lineMovements: LineMovement[];
  summary: EdgeSummary;
}

export interface ScanConfig {
  apiKey: string;
  sports: string[];
  minEdge: number;
  bankroll: number;
  kellyFraction: number;
  regions: string;
  markets: string;
}
