import { useState } from 'react'
import type { MockRiskEvent } from '../lib/mockRiskData'
import { RiskLevelBadge } from './RiskLevelBadge'

interface RiskCardProps {
  event: MockRiskEvent
  reminderOn: boolean
  onToggleReminder: () => void
}

const FILL_CLASS_BY_LEVEL: Record<MockRiskEvent['riskLevel'], string> = {
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
            {event.category} · {event.when} · {event.distanceLabel}
          </div>
        </div>
        <RiskLevelBadge level={event.riskLevel} />
      </div>

      <div className="progress-row">
        <span>Parking pressure</span>
        <span>{event.parkingPressurePercent}%</span>
      </div>
      <div className="progress-track">
        <div
          className={FILL_CLASS_BY_LEVEL[event.riskLevel]}
          style={{ width: `${event.parkingPressurePercent}%` }}
        />
      </div>

      <p className="risk-reason">{event.reason}</p>

      <div className="risk-card-footer">
        <label className="toggle">
          <input type="checkbox" checked={reminderOn} onChange={onToggleReminder} />
          <span className="toggle-track">
            <span className="toggle-thumb" />
          </span>
          <span>Remind me</span>
        </label>

        <button type="button" className="btn-text" onClick={handleSimulateAlert} disabled={!reminderOn}>
          {alertSimulated ? 'Alert simulated ✓' : 'Simulate alert'}
        </button>
      </div>
    </div>
  )
}
