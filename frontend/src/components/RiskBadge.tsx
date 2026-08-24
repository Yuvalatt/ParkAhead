import type { RiskLevel } from '../types'

const COLOR_BY_LEVEL: Record<RiskLevel, string> = {
  Low: 'var(--low)',
  Medium: 'var(--medium)',
  High: 'var(--high)',
}

export function RiskBadge({ level }: { level: RiskLevel }) {
  return (
    <span
      style={{
        display: 'inline-block',
        padding: '2px 10px',
        borderRadius: 999,
        fontSize: 13,
        fontWeight: 600,
        color: '#fff',
        background: COLOR_BY_LEVEL[level],
      }}
    >
      {level}
    </span>
  )
}
