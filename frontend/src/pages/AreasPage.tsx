import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/client'
import { CreateAreaForm } from '../components/CreateAreaForm'
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

  function handleCreated(area: MonitoredArea) {
    setAreas((current) => [...current, area].sort((a, b) => a.name.localeCompare(b.name)))
  }

  return (
    <div>
      <h1>ParkAhead</h1>
      <p>Monitored areas and their upcoming parking-risk forecasts.</p>

      <h2>Add a monitored area</h2>
      <CreateAreaForm onCreated={handleCreated} />

      <h2 style={{ marginTop: 32 }}>Your monitored areas</h2>

      {loading && <p>Loading…</p>}
      {error && <p className="error-text">{error}</p>}
      {!loading && !error && areas.length === 0 && <p>No monitored areas yet.</p>}

      <ul style={{ listStyle: 'none', padding: 0, display: 'grid', gap: 10 }}>
        {areas.map((area) => (
          <li key={area.id} className="card">
            <Link to={`/areas/${area.id}`}>
              <strong>{area.name}</strong>
            </Link>
            <div style={{ fontSize: 14, color: 'var(--text)' }}>{area.address}</div>
            <div style={{ fontSize: 13, color: 'var(--text)' }}>Radius: {area.radiusMeters} m</div>
          </li>
        ))}
      </ul>
    </div>
  )
}
