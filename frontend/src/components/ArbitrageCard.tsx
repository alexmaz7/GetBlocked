import React from 'react';
import { ArbitrageBet } from '../types';
import RatingBadge from './RatingBadge';

interface Props {
  arb: ArbitrageBet;
}

const pct = (v: number) => `${(v * 100).toFixed(2)}%`;
const odds = (v: number) => (v > 0 ? `+${v}` : `${v}`);

const ArbitrageCard: React.FC<Props> = ({ arb }) => (
  <div style={{
    background: '#1e293b',
    border: '1px solid #8b5cf6',
    borderRadius: '12px',
    padding: '20px',
    marginBottom: '12px',
  }}>
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        <span style={{ color: '#8b5cf6', fontWeight: 700, fontSize: '0.85rem' }}>ARBITRAGE</span>
        <RatingBadge rating={arb.rating} />
      </div>
      <span style={{ color: '#64748b', fontSize: '0.8rem' }}>{arb.league}</span>
    </div>

    <div style={{ color: '#f1f5f9', fontWeight: 600, fontSize: '1.1rem', marginBottom: '4px' }}>
      {arb.matchup}
    </div>
    <div style={{ color: '#cbd5e1', fontSize: '0.9rem', marginBottom: '12px' }}>
      <span style={{ color: '#64748b' }}>Market: </span>{arb.market}
      <span style={{ color: '#64748b', marginLeft: '16px' }}>Guaranteed Profit: </span>
      <strong style={{ color: '#8b5cf6', fontSize: '1.1rem' }}>{pct(arb.profitMargin)}</strong>
    </div>

    <div style={{ display: 'flex', flexDirection: 'column', gap: '6px' }}>
      {arb.legs.map((leg, i) => (
        <div key={i} style={{
          background: '#0f172a',
          borderRadius: '8px',
          padding: '10px 14px',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          color: '#cbd5e1',
          fontSize: '0.85rem',
        }}>
          <span>
            <strong>{leg.outcome}</strong>
            <span style={{ color: '#64748b' }}> @ </span>
            {odds(leg.americanOdds)}
            <span style={{ color: '#64748b' }}> ({leg.sportsbook})</span>
          </span>
          <span>
            <span style={{ color: '#64748b' }}>Stake: </span>
            <strong>{pct(leg.stakePercent)}</strong>
          </span>
        </div>
      ))}
    </div>
  </div>
);

export default ArbitrageCard;
