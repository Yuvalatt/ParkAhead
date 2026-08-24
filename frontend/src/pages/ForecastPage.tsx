import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { api } from '../api/client'
import { RiskBadge } from '../components/RiskBadge'
import type { RiskForecastDay } from '../types'

export function ForecastPage() {
  const { areaId } = useParams<{ areaId: string }>()
  const [forecast, setForecast] = useState<RiskForecastDay[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!areaId) return
    api
      .getRiskForecast(areaId)
      .then(setForecast)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false))
  }, [areaId])

  return (
    <div>
      <h1>Risk forecast</h1>

      {loading && <p>Loading…</p>}
      {error && <p style={{ color: 'var(--high)' }}>{error}</p>}

      {forecast.map((day) => (
        <div key={day.date} style={{ border: '1px solid var(--border)', borderRadius: 8, padding: 12, marginBottom: 8 }}>
          <strong>{day.date}</strong> <RiskBadge level={day.riskLevel} /> (score: {day.riskScore})
          <ul>
            {day.contributingEvents.map((event) => (
              <li key={event.eventId}>{event.title} — {event.reason}</li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  )
}
