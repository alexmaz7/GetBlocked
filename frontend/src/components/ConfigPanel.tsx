import React, { useState } from 'react';

interface Props {
  onScanDemo: (bankroll: number, minEdge: number, kellyFraction: number) => void;
  onScanLive: (apiKey: string, sports: string[], bankroll: number, minEdge: number, kellyFraction: number) => void;
  loading: boolean;
}

const inputStyle: React.CSSProperties = {
  background: '#0f172a',
  border: '1px solid #334155',
  borderRadius: '8px',
  padding: '8px 12px',
  color: '#f1f5f9',
  fontSize: '0.9rem',
  width: '100%',
  boxSizing: 'border-box',
};

const labelStyle: React.CSSProperties = {
  color: '#94a3b8',
  fontSize: '0.8rem',
  marginBottom: '4px',
  display: 'block',
};

const SPORTS_OPTIONS = [
  { key: 'americanfootball_nfl', label: 'NFL' },
  { key: 'basketball_nba', label: 'NBA' },
  { key: 'baseball_mlb', label: 'MLB' },
  { key: 'icehockey_nhl', label: 'NHL' },
  { key: 'americanfootball_ncaaf', label: 'NCAAF' },
  { key: 'basketball_ncaab', label: 'NCAAB' },
  { key: 'soccer_epl', label: 'EPL' },
  { key: 'soccer_usa_mls', label: 'MLS' },
  { key: 'mma_mixed_martial_arts', label: 'MMA' },
];

const ConfigPanel: React.FC<Props> = ({ onScanDemo, onScanLive, loading }) => {
  const [apiKey, setApiKey] = useState('');
  const [bankroll, setBankroll] = useState(1000);
  const [minEdge, setMinEdge] = useState(2);
  const [kellyFraction, setKellyFraction] = useState(25);
  const [selectedSports, setSelectedSports] = useState<string[]>(['americanfootball_nfl', 'basketball_nba']);

  const toggleSport = (key: string) => {
    setSelectedSports(prev =>
      prev.includes(key) ? prev.filter(s => s !== key) : [...prev, key]
    );
  };

  return (
    <div style={{
      background: '#1e293b',
      borderRadius: '12px',
      padding: '20px',
      marginBottom: '24px',
    }}>
      <h3 style={{ color: '#f1f5f9', margin: '0 0 16px', fontSize: '1rem' }}>Configuration</h3>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginBottom: '16px' }}>
        <div>
          <label style={labelStyle}>Bankroll ($)</label>
          <input
            type="number"
            value={bankroll}
            onChange={e => setBankroll(Number(e.target.value))}
            style={inputStyle}
          />
        </div>
        <div>
          <label style={labelStyle}>Min Edge (%)</label>
          <input
            type="number"
            step="0.5"
            value={minEdge}
            onChange={e => setMinEdge(Number(e.target.value))}
            style={inputStyle}
          />
        </div>
        <div>
          <label style={labelStyle}>Kelly Fraction (%)</label>
          <input
            type="number"
            step="5"
            value={kellyFraction}
            onChange={e => setKellyFraction(Number(e.target.value))}
            style={inputStyle}
          />
        </div>
        <div>
          <label style={labelStyle}>API Key (for live data)</label>
          <input
            type="password"
            value={apiKey}
            onChange={e => setApiKey(e.target.value)}
            placeholder="the-odds-api.com key"
            style={inputStyle}
          />
        </div>
      </div>

      <div style={{ marginBottom: '16px' }}>
        <label style={labelStyle}>Sports</label>
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
          {SPORTS_OPTIONS.map(sport => (
            <button
              key={sport.key}
              onClick={() => toggleSport(sport.key)}
              style={{
                padding: '6px 14px',
                borderRadius: '9999px',
                border: 'none',
                fontSize: '0.8rem',
                fontWeight: 600,
                cursor: 'pointer',
                background: selectedSports.includes(sport.key) ? '#e94560' : '#334155',
                color: selectedSports.includes(sport.key) ? '#fff' : '#94a3b8',
                transition: 'all 0.2s',
              }}
            >
              {sport.label}
            </button>
          ))}
        </div>
      </div>

      <div style={{ display: 'flex', gap: '12px' }}>
        <button
          onClick={() => onScanDemo(bankroll, minEdge / 100, kellyFraction / 100)}
          disabled={loading}
          style={{
            padding: '10px 24px',
            borderRadius: '8px',
            border: 'none',
            background: '#e94560',
            color: '#fff',
            fontWeight: 700,
            fontSize: '0.9rem',
            cursor: loading ? 'not-allowed' : 'pointer',
            opacity: loading ? 0.6 : 1,
          }}
        >
          {loading ? 'Scanning...' : 'Scan Demo Data'}
        </button>
        <button
          onClick={() => onScanLive(apiKey, selectedSports, bankroll, minEdge / 100, kellyFraction / 100)}
          disabled={loading || !apiKey}
          style={{
            padding: '10px 24px',
            borderRadius: '8px',
            border: '1px solid #e94560',
            background: 'transparent',
            color: '#e94560',
            fontWeight: 700,
            fontSize: '0.9rem',
            cursor: (loading || !apiKey) ? 'not-allowed' : 'pointer',
            opacity: (loading || !apiKey) ? 0.5 : 1,
          }}
        >
          {loading ? 'Scanning...' : 'Scan Live Odds'}
        </button>
      </div>
    </div>
  );
};

export default ConfigPanel;
