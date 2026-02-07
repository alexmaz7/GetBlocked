import React, { useState } from 'react';
import Header from './components/Header';
import ConfigPanel from './components/ConfigPanel';
import EdgeDashboard from './components/EdgeDashboard';
import { EdgeResult } from './types';
import { fetchDemoEdges, scanEdges } from './services/api';
import './App.css';

function App() {
  const [result, setResult] = useState<EdgeResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleScanDemo = async (bankroll: number, minEdge: number, kellyFraction: number) => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchDemoEdges(minEdge, bankroll, kellyFraction);
      setResult(data);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch demo data');
    } finally {
      setLoading(false);
    }
  };

  const handleScanLive = async (
    apiKey: string,
    sports: string[],
    bankroll: number,
    minEdge: number,
    kellyFraction: number
  ) => {
    setLoading(true);
    setError(null);
    try {
      const data = await scanEdges({
        apiKey,
        sports,
        bankroll,
        minEdge,
        kellyFraction,
        regions: 'us',
        markets: 'h2h,spreads,totals',
      });
      setResult(data);
    } catch (err: any) {
      setError(err.message || 'Failed to scan live odds');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', background: '#0f172a' }}>
      <Header />
      <main style={{ maxWidth: '1200px', margin: '0 auto', padding: '24px' }}>
        <ConfigPanel
          onScanDemo={handleScanDemo}
          onScanLive={handleScanLive}
          loading={loading}
        />

        {error && (
          <div style={{
            background: '#7f1d1d',
            border: '1px solid #dc2626',
            borderRadius: '8px',
            padding: '12px 16px',
            color: '#fca5a5',
            marginBottom: '16px',
            fontSize: '0.9rem',
          }}>
            {error}
          </div>
        )}

        {result && <EdgeDashboard result={result} />}

        {!result && !loading && (
          <div style={{
            textAlign: 'center',
            padding: '60px 20px',
            color: '#475569',
          }}>
            <h2 style={{ fontSize: '1.5rem', marginBottom: '8px', color: '#64748b' }}>
              Ready to find edges
            </h2>
            <p style={{ fontSize: '0.95rem' }}>
              Click "Scan Demo Data" to try with simulated odds, or enter your API key for live data.
            </p>
          </div>
        )}
      </main>
    </div>
  );
}

export default App;
