#!/usr/bin/env node
/**
 * Standalone demo scanner — runs edge detection with simulated data.
 * Same algorithms as the C# backend, portable Node.js version.
 */

// ── Odds Calculator ──────────────────────────────────────────────

function americanToDecimal(american) {
  return american > 0 ? american / 100 + 1 : 100 / Math.abs(american) + 1;
}

function impliedProb(decimal) {
  return decimal <= 1 ? 1 : 1 / decimal;
}

function powerDevig(probs) {
  if (probs.length !== 2) {
    const total = probs.reduce((s, p) => s + p, 0);
    return probs.map(p => p / total);
  }
  let lo = 0.01, hi = 5;
  for (let i = 0; i < 100; i++) {
    const mid = (lo + hi) / 2;
    Math.pow(probs[0], mid) + Math.pow(probs[1], mid) > 1 ? lo = mid : hi = mid;
  }
  const k = (lo + hi) / 2;
  return [Math.pow(probs[0], k), Math.pow(probs[1], k)];
}

// ── Kelly ────────────────────────────────────────────────────────

function kellyStake(trueProb, decimalOdds, bankroll, fraction = 0.25) {
  const b = decimalOdds - 1;
  const full = Math.max(0, Math.min(1, (b * trueProb - (1 - trueProb)) / b));
  return Math.round(full * fraction * bankroll * 100) / 100;
}

// ── Demo Data ────────────────────────────────────────────────────

function book(name, american) {
  const dec = americanToDecimal(american);
  return { sportsbook: name, american, decimal: Math.round(dec * 1000) / 1000, implied: Math.round(impliedProb(dec) * 10000) / 10000 };
}

function makeOutcome(name, books) {
  const best = books.reduce((b, x) => x.decimal > b.decimal ? x : b, books[0]);
  const consensus = books.reduce((s, b) => s + b.implied, 0) / books.length;
  return { name, books, best, consensus: Math.round(consensus * 10000) / 10000 };
}

const events = [
  {
    id: 'nfl-1', league: 'NFL', home: 'Kansas City Chiefs', away: 'Buffalo Bills',
    time: new Date(Date.now() + 3 * 3600000).toISOString(),
    markets: [{
      key: 'h2h', label: 'Moneyline',
      outcomes: [
        makeOutcome('Kansas City Chiefs', [book('DraftKings', -155), book('FanDuel', -150), book('BetMGM', -148), book('Caesars', -130), book('PointsBet', -152)]),
        makeOutcome('Buffalo Bills', [book('DraftKings', 135), book('FanDuel', 130), book('BetMGM', 128), book('Caesars', 110), book('PointsBet', 132)]),
      ]
    }, {
      key: 'spreads', label: 'Spread',
      outcomes: [
        makeOutcome('KC Chiefs -3.5', [book('DraftKings', -110), book('FanDuel', -108), book('BetMGM', -112), book('Caesars', -105), book('PointsBet', -110)]),
        makeOutcome('BUF Bills +3.5', [book('DraftKings', -110), book('FanDuel', -112), book('BetMGM', -108), book('Caesars', -115), book('PointsBet', -110)]),
      ]
    }]
  },
  {
    id: 'nba-1', league: 'NBA', home: 'Boston Celtics', away: 'Milwaukee Bucks',
    time: new Date(Date.now() + 5 * 3600000).toISOString(),
    markets: [{
      key: 'h2h', label: 'Moneyline',
      outcomes: [
        makeOutcome('Boston Celtics', [book('DraftKings', -135), book('FanDuel', -140), book('BetMGM', -125), book('Caesars', -138), book('PointsBet', -142)]),
        makeOutcome('Milwaukee Bucks', [book('DraftKings', 115), book('FanDuel', 120), book('BetMGM', 105), book('Caesars', 118), book('PointsBet', 130)]),
      ]
    }, {
      key: 'totals', label: 'Totals',
      outcomes: [
        makeOutcome('Over 224.5', [book('DraftKings', -108), book('FanDuel', -110), book('BetMGM', -105), book('Caesars', -110), book('PointsBet', -115)]),
        makeOutcome('Under 224.5', [book('DraftKings', -112), book('FanDuel', -110), book('BetMGM', -115), book('Caesars', -110), book('PointsBet', -105)]),
      ]
    }]
  },
  {
    id: 'epl-1', league: 'English Premier League', home: 'Arsenal', away: 'Manchester City',
    time: new Date(Date.now() + 24 * 3600000).toISOString(),
    markets: [{
      key: 'h2h', label: 'Moneyline',
      outcomes: [
        makeOutcome('Arsenal', [book('DraftKings', 165), book('FanDuel', 160), book('BetMGM', 170), book('Bet365', 185), book('William Hill', 155)]),
        makeOutcome('Manchester City', [book('DraftKings', 150), book('FanDuel', 155), book('BetMGM', 145), book('Bet365', 130), book('William Hill', 160)]),
        makeOutcome('Draw', [book('DraftKings', 240), book('FanDuel', 235), book('BetMGM', 245), book('Bet365', 250), book('William Hill', 230)]),
      ]
    }]
  },
  {
    id: 'nhl-1', league: 'NHL', home: 'Toronto Maple Leafs', away: 'Montreal Canadiens',
    time: new Date(Date.now() + 7 * 3600000).toISOString(),
    markets: [{
      key: 'h2h', label: 'Moneyline',
      outcomes: [
        makeOutcome('Toronto Maple Leafs', [book('DraftKings', -180), book('FanDuel', -175), book('BetMGM', -185), book('Caesars', -160), book('PointsBet', -178)]),
        makeOutcome('Montreal Canadiens', [book('DraftKings', 155), book('FanDuel', 150), book('BetMGM', 160), book('Caesars', 140), book('PointsBet', 153)]),
      ]
    }, {
      key: 'totals', label: 'Totals',
      outcomes: [
        makeOutcome('Over 6.5', [book('DraftKings', 105), book('FanDuel', -102), book('BetMGM', 110), book('Caesars', -105), book('PointsBet', 108)]),
        makeOutcome('Under 6.5', [book('DraftKings', -125), book('FanDuel', -118), book('BetMGM', -130), book('Caesars', -115), book('PointsBet', -128)]),
      ]
    }]
  },
];

// ── Edge Detection ───────────────────────────────────────────────

const C = {
  reset: '\x1b[0m', bold: '\x1b[1m', dim: '\x1b[2m',
  red: '\x1b[31m', green: '\x1b[32m', yellow: '\x1b[33m',
  blue: '\x1b[34m', magenta: '\x1b[35m', cyan: '\x1b[36m',
  bgGreen: '\x1b[42m', bgYellow: '\x1b[43m', bgRed: '\x1b[41m',
};

const BANKROLL = 1000;
const MIN_EDGE = 0.02;
const KELLY_FRAC = 0.25;

function rateEdge(e) { return e >= 0.08 ? 'HIGH' : e >= 0.04 ? 'MED ' : 'LOW '; }
function rateBg(e) { return e >= 0.08 ? C.bgGreen : e >= 0.04 ? C.bgYellow : C.bgRed; }
function pct(v) { return (v * 100).toFixed(2) + '%'; }
function odds(v) { return (v > 0 ? '+' : '') + v; }
function fmtTime(iso) {
  return new Date(iso).toLocaleString('en-US', { month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' });
}

console.log(`
${C.bold}${C.green}  ██████╗ ███████╗████████╗██████╗ ██╗      ██████╗  ██████╗██╗  ██╗███████╗██████╗
 ██╔════╝ ██╔════╝╚══██╔══╝██╔══██╗██║     ██╔═══██╗██╔════╝██║ ██╔╝██╔════╝██╔══██╗
 ██║  ███╗█████╗     ██║   ██████╔╝██║     ██║   ██║██║     █████╔╝ █████╗  ██║  ██║
 ██║   ██║██╔══╝     ██║   ██╔══██╗██║     ██║   ██║██║     ██╔═██╗ ██╔══╝  ██║  ██║
 ╚██████╔╝███████╗   ██║   ██████╔╝███████╗╚██████╔╝╚██████╗██║  ╚██╗███████╗██████╔╝
  ╚═════╝ ╚══════╝   ╚═╝   ╚═════╝ ╚══════╝ ╚═════╝  ╚═════╝╚═╝   ╚═╝╚══════╝╚═════╝${C.reset}
${C.dim}  Sports Betting Edge Finder — Demo Scan${C.reset}
`);

console.log(`${C.cyan}  Bankroll: $${BANKROLL}  Min edge: ${pct(MIN_EDGE)}  Kelly: ${KELLY_FRAC * 100}%${C.reset}`);
console.log(`${C.dim}  Scanning ${events.length} events across ${new Set(events.map(e => e.league)).size} leagues...${C.reset}\n`);

const sep = `${C.dim}  ${'─'.repeat(70)}${C.reset}`;
let valueBets = [], arbs = [], lineMovements = [];

for (const evt of events) {
  const matchup = `${evt.away} @ ${evt.home}`;
  const matchupLine = `${C.bold}${matchup}${C.reset} ${C.dim}(${evt.league} · ${fmtTime(evt.time)})${C.reset}`;

  for (const market of evt.markets) {
    // ── Value Bets ──
    const avgProbs = market.outcomes.map(o => o.books.reduce((s, b) => s + b.implied, 0) / o.books.length);
    const devigged = market.outcomes.length === 2 ? powerDevig(avgProbs) : (() => { const t = avgProbs.reduce((s,p)=>s+p,0); return avgProbs.map(p=>p/t); })();

    market.outcomes.forEach((outcome, i) => {
      const trueProb = devigged[i];
      for (const bk of outcome.books) {
        const edge = trueProb - bk.implied;
        if (edge >= MIN_EDGE) {
          const stake = kellyStake(trueProb, bk.decimal, BANKROLL, KELLY_FRAC);
          valueBets.push({ matchup: matchupLine, market: market.label, outcome: outcome.name, book: bk, trueProb, edge, stake, rating: rateEdge(edge) });
        }
      }
    });

    // ── Arbitrage ──
    if (market.outcomes.length >= 2) {
      const bests = market.outcomes.map(o => o.best);
      const totalImpl = bests.reduce((s, b) => s + b.implied, 0);
      if (totalImpl < 1) {
        const profit = 1 - totalImpl;
        arbs.push({ matchup: matchupLine, market: market.label, legs: market.outcomes.map((o, i) => ({
          outcome: o.name, book: bests[i], stakeP: bests[i].implied / totalImpl
        })), totalImpl, profit, rating: rateEdge(profit) });
      }
    }

    // ── Line Movement ──
    for (const outcome of market.outcomes) {
      if (outcome.books.length < 3) continue;
      const probs = outcome.books.map(b => b.implied);
      const avg = probs.reduce((s,p) => s+p, 0) / probs.length;
      const worst = Math.max(...probs);
      const spread = worst - outcome.best.implied;
      if (spread >= 0.03) {
        const dir = outcome.best.implied < avg - 0.015 ? 'STEAM' : 'REVERSE';
        lineMovements.push({ matchup: matchupLine, market: market.label, outcome: outcome.name, best: outcome.best, spread, dir, rating: rateEdge(spread) });
      }
    }
  }
}

// Sort by edge magnitude
valueBets.sort((a, b) => b.edge - a.edge);
arbs.sort((a, b) => b.profit - a.profit);
lineMovements.sort((a, b) => b.spread - a.spread);

const total = valueBets.length + arbs.length + lineMovements.length;
console.log(`${C.bold}${C.green}  ⚡ Found ${total} edge${total !== 1 ? 's' : ''}${C.reset}\n`);
console.log(sep);

// Print value bets
for (const vb of valueBets) {
  console.log(`  ${rateBg(vb.edge)}${C.bold} ${vb.rating} ${C.reset} ${C.green}VALUE BET${C.reset}`);
  console.log(`  ${vb.matchup}`);
  console.log(`  ${C.cyan}Market:${C.reset} ${vb.market} — ${C.bold}${vb.outcome}${C.reset}`);
  console.log(`  ${C.cyan}Book:${C.reset} ${vb.book.sportsbook} @ ${C.bold}${odds(vb.book.american)}${C.reset} (${vb.book.decimal})`);
  console.log(`  ${C.cyan}Book implied:${C.reset} ${pct(vb.book.implied)}  ${C.cyan}True prob:${C.reset} ${pct(vb.trueProb)}`);
  console.log(`  ${C.cyan}Edge:${C.reset} ${C.green}${C.bold}${pct(vb.edge)}${C.reset}  ${C.cyan}Kelly stake:${C.reset} $${vb.stake.toFixed(2)}`);
  console.log(sep);
}

// Print arbitrage
for (const arb of arbs) {
  console.log(`  ${rateBg(arb.profit)}${C.bold} ${arb.rating} ${C.reset} ${C.magenta}ARBITRAGE${C.reset}`);
  console.log(`  ${arb.matchup}`);
  console.log(`  ${C.cyan}Market:${C.reset} ${arb.market}  ${C.cyan}Profit:${C.reset} ${C.magenta}${C.bold}${pct(arb.profit)}${C.reset}`);
  for (const leg of arb.legs) {
    console.log(`  ${C.cyan}Leg:${C.reset} ${leg.outcome} @ ${C.bold}${odds(leg.book.american)}${C.reset} (${leg.book.sportsbook}) — Stake: ${pct(leg.stakeP)}`);
  }
  console.log(sep);
}

// Print line movements
for (const lm of lineMovements) {
  const dirLabel = lm.dir === 'STEAM' ? 'STEAM MOVE' : 'REVERSE LINE';
  console.log(`  ${rateBg(lm.spread)}${C.bold} ${lm.rating} ${C.reset} ${C.yellow}${dirLabel}${C.reset}`);
  console.log(`  ${lm.matchup}`);
  console.log(`  ${C.cyan}Market:${C.reset} ${lm.market} — ${C.bold}${lm.outcome}${C.reset}`);
  console.log(`  ${C.cyan}Current best:${C.reset} ${odds(lm.best.american)} (${lm.best.sportsbook})  ${C.cyan}Spread:${C.reset} ${C.yellow}${pct(lm.spread)}${C.reset}`);
  console.log(sep);
}

// Summary
const totalKelly = valueBets.reduce((s, v) => s + v.stake, 0);
console.log(`\n${C.bold}  SUMMARY${C.reset}`);
console.log(`  ${C.cyan}Value bets:${C.reset} ${valueBets.length}   ${C.cyan}Arbitrage:${C.reset} ${arbs.length}   ${C.cyan}Line signals:${C.reset} ${lineMovements.length}`);
console.log(`  ${C.cyan}Total Kelly exposure:${C.reset} $${totalKelly.toFixed(2)} of $${BANKROLL} bankroll`);
if (valueBets.length > 0)
  console.log(`  ${C.cyan}Best edge:${C.reset} ${valueBets[0].outcome} (${valueBets[0].market}) @ ${valueBets[0].book.sportsbook} — ${pct(valueBets[0].edge)} edge`);
if (arbs.length > 0)
  console.log(`  ${C.cyan}Best arb:${C.reset} ${arbs[0].market} — ${pct(arbs[0].profit)} guaranteed profit`);
console.log('');
