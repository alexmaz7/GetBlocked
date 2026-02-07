import React from 'react';

interface Props {
  rating: 'low' | 'medium' | 'high';
}

const colors: Record<string, { bg: string; text: string }> = {
  high: { bg: '#059669', text: '#fff' },
  medium: { bg: '#d97706', text: '#fff' },
  low: { bg: '#6b7280', text: '#fff' },
};

const RatingBadge: React.FC<Props> = ({ rating }) => (
  <span style={{
    display: 'inline-block',
    padding: '2px 10px',
    borderRadius: '9999px',
    fontSize: '0.7rem',
    fontWeight: 700,
    textTransform: 'uppercase',
    letterSpacing: '0.5px',
    backgroundColor: colors[rating].bg,
    color: colors[rating].text,
  }}>
    {rating}
  </span>
);

export default RatingBadge;
