import React from 'react';
import { EdgeSummary } from '../types';

interface Props {
  summary: EdgeSummary;
}

const cardStyle: React.CSSProperties = {
  background: '#1e293b',
  borderRadius: '12px',
  padding: '20px',
  flex: '1 1 180px',
  minWidth: '180px',
  textAlign: 'center',
};

const labelStyle: React.CSSProperties = {
  color: '#94a3b8',
  fontSize: '0.8rem',
  textTransform: 'uppercase',
  letterSpacing: '1px',
  marginBottom: '4px',
};

const valueStyle: React.CSSProperties = {
  fontSize: '2rem',
  fontWeight: 700,
  color: '#f1f5f9',
};

const SummaryCards: React.FC<Props> = ({ summary }) => (
  <div style={{ display: 'flex', gap: '16px', flexWrap: 'wrap', marginBottom: '24px' }}>
    <div style={cardStyle}>
      <div style={labelStyle}>Total Edges</div>
      <div style={{ ...valueStyle, color: '#e94560' }}>{summary.totalEdges}</div>
    </div>
    <div style={cardStyle}>
      <div style={labelStyle}>Value Bets</div>
      <div style={{ ...valueStyle, color: '#10b981' }}>{summary.valueBetCount}</div>
    </div>
    <div style={cardStyle}>
      <div style={labelStyle}>Arbitrage</div>
      <div style={{ ...valueStyle, color: '#8b5cf6' }}>{summary.arbitrageCount}</div>
    </div>
    <div style={cardStyle}>
      <div style={labelStyle}>Line Movement</div>
      <div style={{ ...valueStyle, color: '#f59e0b' }}>{summary.lineMovementCount}</div>
    </div>
    <div style={cardStyle}>
      <div style={labelStyle}>Events Scanned</div>
      <div style={valueStyle}>{summary.eventsScanned}</div>
    </div>
    <div style={cardStyle}>
      <div style={labelStyle}>Kelly Exposure</div>
      <div style={{ ...valueStyle, fontSize: '1.5rem' }}>
        ${summary.totalKellyExposure.toFixed(2)}
      </div>
      <div style={{ color: '#64748b', fontSize: '0.75rem' }}>
        of ${summary.bankroll.toFixed(0)}
      </div>
    </div>
  </div>
);

export default SummaryCards;
