import React, { useState } from 'react';
import { EdgeResult } from '../types';
import SummaryCards from './SummaryCards';
import ValueBetCard from './ValueBetCard';
import ArbitrageCard from './ArbitrageCard';
import LineMovementCard from './LineMovementCard';

interface Props {
  result: EdgeResult;
}

type Tab = 'all' | 'value' | 'arbitrage' | 'line_movement';

const tabs: { key: Tab; label: string; color: string }[] = [
  { key: 'all', label: 'All Edges', color: '#e94560' },
  { key: 'value', label: 'Value Bets', color: '#10b981' },
  { key: 'arbitrage', label: 'Arbitrage', color: '#8b5cf6' },
  { key: 'line_movement', label: 'Line Movement', color: '#f59e0b' },
];

const EdgeDashboard: React.FC<Props> = ({ result }) => {
  const [activeTab, setActiveTab] = useState<Tab>('all');

  const showValue = activeTab === 'all' || activeTab === 'value';
  const showArbitrage = activeTab === 'all' || activeTab === 'arbitrage';
  const showLineMovement = activeTab === 'all' || activeTab === 'line_movement';

  const totalVisible =
    (showValue ? result.valueBets.length : 0) +
    (showArbitrage ? result.arbitrageBets.length : 0) +
    (showLineMovement ? result.lineMovements.length : 0);

  return (
    <div>
      <SummaryCards summary={result.summary} />

      <div style={{ display: 'flex', gap: '8px', marginBottom: '20px', flexWrap: 'wrap' }}>
        {tabs.map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            style={{
              padding: '8px 20px',
              borderRadius: '8px',
              border: activeTab === tab.key ? `2px solid ${tab.color}` : '2px solid transparent',
              background: activeTab === tab.key ? `${tab.color}22` : '#1e293b',
              color: activeTab === tab.key ? tab.color : '#94a3b8',
              fontWeight: 600,
              fontSize: '0.85rem',
              cursor: 'pointer',
              transition: 'all 0.2s',
            }}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {totalVisible === 0 && (
        <div style={{
          textAlign: 'center',
          padding: '40px',
          color: '#64748b',
          fontSize: '1rem',
        }}>
          No edges found in this category. Try adjusting your filters.
        </div>
      )}

      {showValue && result.valueBets.map((bet, i) => (
        <ValueBetCard key={`vb-${i}`} bet={bet} />
      ))}

      {showArbitrage && result.arbitrageBets.map((arb, i) => (
        <ArbitrageCard key={`arb-${i}`} arb={arb} />
      ))}

      {showLineMovement && result.lineMovements.map((lm, i) => (
        <LineMovementCard key={`lm-${i}`} movement={lm} />
      ))}
    </div>
  );
};

export default EdgeDashboard;
