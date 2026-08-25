import type { RiskLevel } from '../types'

const CLASS_BY_LEVEL: Record<RiskLevel, string> = {
  Low: 'risk-badge risk-badge--low',
  Medium: 'risk-badge risk-badge--medium',
  High: 'risk-badge risk-badge--high',
}

export function RiskLevelBadge({ level }: { level: RiskLevel }) {
  return <span className={CLASS_BY_LEVEL[level]}>{level} risk</span>
}
