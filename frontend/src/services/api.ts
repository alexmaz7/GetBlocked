import { EdgeResult, ScanConfig } from '../types';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5000';

export async function fetchDemoEdges(
  minEdge: number = 0.02,
  bankroll: number = 1000,
  kellyFraction: number = 0.25
): Promise<EdgeResult> {
  const params = new URLSearchParams({
    minEdge: minEdge.toString(),
    bankroll: bankroll.toString(),
    kellyFraction: kellyFraction.toString(),
  });

  const res = await fetch(`${API_BASE}/api/edges/demo?${params}`);
  if (!res.ok) throw new Error(`API error: ${res.status}`);
  return res.json();
}

export async function scanEdges(config: ScanConfig): Promise<EdgeResult> {
  const res = await fetch(`${API_BASE}/api/edges/scan`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(config),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `API error: ${res.status}`);
  }
  return res.json();
}
