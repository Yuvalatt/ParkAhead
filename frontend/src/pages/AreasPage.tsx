import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/client'
import type { MonitoredArea } from '../types'

export function AreasPage() {
  const [areas, setAreas] = useState<MonitoredArea[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    api
      .getMonitoredAreas()
      .then(setAreas)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  return (
    <div>
      <h1>ParkAhead</h1>
      <p>Monitored areas and their upcoming parking-risk forecasts.</p>

      {loading && <p>Loading…</p>}
      {error && <p style={{ color: 'var(--high)' }}>{error}</p>}

      <ul>
        {areas.map((area) => (
          <li key={area.id}>
            <Link to={`/areas/${area.id}`}>{area.name}</Link>
          </li>
        ))}
      </ul>

      {/* TODO: create-area form */}
    </div>
  )
}
