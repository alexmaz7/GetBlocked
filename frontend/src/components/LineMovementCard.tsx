import React from 'react';
import { LineMovement } from '../types';
import RatingBadge from './RatingBadge';

interface Props {
  movement: LineMovement;
}

const pct = (v: number) => `${(v * 100).toFixed(2)}%`;
const odds = (v: number) => (v > 0 ? `+${v}` : `${v}`);

const LineMovementCard: React.FC<Props> = ({ movement }) => (
  <div style={{
    background: '#1e293b',
    border: '1px solid #f59e0b',
    borderRadius: '12px',
    padding: '20px',
    marginBottom: '12px',
  }}>
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '12px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        <span style={{ color: '#f59e0b', fontWeight: 700, fontSize: '0.85rem' }}>
          {movement.direction === 'steam' ? 'STEAM MOVE' : 'REVERSE LINE'}
        </span>
        <RatingBadge rating={movement.rating} />
      </div>
      <span style={{ color: '#64748b', fontSize: '0.8rem' }}>{movement.league}</span>
    </div>

    <div style={{ color: '#f1f5f9', fontWeight: 600, fontSize: '1.1rem', marginBottom: '8px' }}>
      {movement.matchup}
    </div>

    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '12px', color: '#cbd5e1', fontSize: '0.9rem' }}>
      <div>
        <span style={{ color: '#64748b' }}>Market: </span>{movement.market} &mdash; <strong>{movement.outcome}</strong>
      </div>
      <div>
        <span style={{ color: '#64748b' }}>Current Best: </span>
        <strong>{odds(movement.currentBestOdds)}</strong>
      </div>
      <div>
        <span style={{ color: '#64748b' }}>Spread: </span>
        <strong style={{ color: '#f59e0b' }}>{pct(movement.magnitude)}</strong>
      </div>
    </div>
  </div>
);

export default LineMovementCard;
