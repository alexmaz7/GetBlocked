import React from 'react';
import { ValueBet } from '../types';
import RatingBadge from './RatingBadge';

interface Props {
  bet: ValueBet;
}

const pct = (v: number) => `${(v * 100).toFixed(2)}%`;
const odds = (v: number) => (v > 0 ? `+${v}` : `${v}`);

const ValueBetCard: React.FC<Props> = ({ bet }) => (
  <div style={{
    background: '#1e293b',
    border: '1px solid #10b981',
    borderRadius: '12px',
    padding: '20px',
    marginBottom: '12px',
  }}>
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        <span style={{ color: '#10b981', fontWeight: 700, fontSize: '0.85rem' }}>VALUE BET</span>
        <RatingBadge rating={bet.rating} />
      </div>
      <span style={{ color: '#64748b', fontSize: '0.8rem' }}>{bet.league}</span>
    </div>

    <div style={{ color: '#f1f5f9', fontWeight: 600, fontSize: '1.1rem', marginBottom: '8px' }}>
      {bet.matchup}
    </div>

    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '12px', color: '#cbd5e1', fontSize: '0.9rem' }}>
      <div>
        <span style={{ color: '#64748b' }}>Market: </span>{bet.market} &mdash; <strong>{bet.outcome}</strong>
      </div>
      <div>
        <span style={{ color: '#64748b' }}>Book: </span>{bet.sportsbook} @ <strong>{odds(bet.americanOdds)}</strong> ({bet.decimalOdds.toFixed(3)})
      </div>
      <div>
        <span style={{ color: '#64748b' }}>Book Implied: </span>{pct(bet.impliedProbability)}
        <span style={{ color: '#64748b', marginLeft: '12px' }}>True Prob: </span>{pct(bet.estimatedTrueProbability)}
      </div>
      <div>
        <span style={{ color: '#64748b' }}>Edge: </span>
        <strong style={{ color: '#10b981', fontSize: '1.1rem' }}>{pct(bet.edge)}</strong>
        <span style={{ color: '#64748b', marginLeft: '12px' }}>Kelly Stake: </span>
        <strong>${bet.kellyStake.toFixed(2)}</strong>
      </div>
    </div>
  </div>
);

export default ValueBetCard;
