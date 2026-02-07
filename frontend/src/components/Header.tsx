import React from 'react';

const Header: React.FC = () => (
  <header style={{
    background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)',
    color: '#fff',
    padding: '24px 32px',
    borderBottom: '3px solid #e94560',
  }}>
    <h1 style={{ margin: 0, fontSize: '2rem', fontWeight: 800, letterSpacing: '-0.5px' }}>
      GetBlocked
    </h1>
    <p style={{ margin: '4px 0 0', color: '#a0aec0', fontSize: '0.9rem' }}>
      Sports Betting Edge Finder &mdash; Value Bets, Arbitrage &amp; Line Movement
    </p>
  </header>
);

export default Header;
