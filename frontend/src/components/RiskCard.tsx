import { useState } from 'react'
import { formatDistance, formatEventTime } from '../lib/format'
import type { EventRisk, RiskLevel } from '../types'
import { RiskLevelBadge } from './RiskLevelBadge'

interface RiskCardProps {
  event: EventRisk
  reminderOn: boolean
  onToggleReminder: () => void
}

const FILL_CLASS_BY_LEVEL: Record<RiskLevel, string> = {
  Low: 'progress-fill progress-fill--low',
  Medium: 'progress-fill progress-fill--medium',
  High: 'progress-fill progress-fill--high',
}

export function RiskCard({ event, reminderOn, onToggleReminder }: RiskCardProps) {
  const [alertSimulated, setAlertSimulated] = useState(false)

  function handleSimulateAlert() {
    setAlertSimulated(true)
    window.setTimeout(() => setAlertSimulated(false), 2500)
  }

  return (
    <div className="risk-card">
      <div className="risk-card-header">
        <div>
          <h3 className="risk-card-title">{event.title}</h3>
          <div className="risk-meta">
            {event.category}
            {event.venueName ? ` · ${event.venueName}` : ''} · {formatEventTime(event.startTime)} ·{' '}
            {formatDistance(event.distanceKm)}
          </div>
        </div>
        <RiskLevelBadge level={event.riskLevel} />
      </div>

      <div className="progress-row">
        <span>Parking pressure</span>
        <span>{event.riskScore}%</span>
      </div>
      <div className="progress-track">
        <div className={FILL_CLASS_BY_LEVEL[event.riskLevel]} style={{ width: `${event.riskScore}%` }} />
      </div>

      <ul className="risk-reasons">
        {event.reasons.map((reason) => (
          <li key={reason}>{reason}</li>
        ))}
      </ul>

      <div className="risk-card-footer">
        <label className="toggle">
          <input type="checkbox" checked={reminderOn} onChange={onToggleReminder} />
          <span className="toggle-track">
            <span className="toggle-thumb" />
          </span>
          <span>Remind me (demo)</span>
        </label>

        <button type="button" className="btn-text" onClick={handleSimulateAlert} disabled={!reminderOn}>
          {alertSimulated ? 'Alert simulated ✓' : 'Simulate alert'}
        </button>
      </div>
    </div>
  )
}
