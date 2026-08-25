import { useEffect, useState } from 'react'
import { api } from '../api/client'
import { CreateAreaForm } from '../components/CreateAreaForm'
import { DashboardHeader } from '../components/DashboardHeader'
import { Modal } from '../components/Modal'
import { RiskCard } from '../components/RiskCard'
import { SummaryCards } from '../components/SummaryCards'
import type { MonitoredArea, RiskForecast } from '../types'

const SELECTED_AREA_STORAGE_KEY = 'parkahead:selectedAreaId'

export function AreasPage() {
  const [areas, setAreas] = useState<MonitoredArea[]>([])
  const [selectedAreaId, setSelectedAreaId] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [modalOpen, setModalOpen] = useState(false)

  // Reminders are frontend-only demo state — there is no backend/notification support yet
  // (see RiskCard). Keyed by the real event ids returned from the forecast, so it resets
  // whenever the forecast changes.
  const [reminders, setReminders] = useState<Record<string, boolean>>({})

  const [forecast, setForecast] = useState<RiskForecast | null>(null)
  const [forecastLoading, setForecastLoading] = useState(false)
  const [forecastError, setForecastError] = useState<string | null>(null)

  useEffect(() => {
    api
      .getMonitoredAreas()
      .then((fetched) => {
        setAreas(fetched)
        const storedId = localStorage.getItem(SELECTED_AREA_STORAGE_KEY)
        const defaultId = fetched.find((a) => a.id === storedId)?.id ?? fetched[0]?.id ?? null
        setSelectedAreaId(defaultId)
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    if (!selectedAreaId) {
      setForecast(null)
      return
    }

    let cancelled = false
    setForecastLoading(true)
    setForecastError(null)
    setReminders({})

    api
      .getRiskForecast(selectedAreaId)
      .then((result) => {
        if (!cancelled) setForecast(result)
      })
      .catch((err: Error) => {
        if (!cancelled) {
          setForecast(null)
          setForecastError(err.message)
        }
      })
      .finally(() => {
        if (!cancelled) setForecastLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [selectedAreaId])

  function selectArea(id: string) {
    setSelectedAreaId(id)
    localStorage.setItem(SELECTED_AREA_STORAGE_KEY, id)
  }

  function handleCreated(area: MonitoredArea) {
    setAreas((current) => [...current, area].sort((a, b) => a.name.localeCompare(b.name)))
    selectArea(area.id)
    setModalOpen(false)
  }

  function toggleReminder(eventId: string) {
    setReminders((current) => ({ ...current, [eventId]: !current[eventId] }))
  }

  const selectedArea = areas.find((a) => a.id === selectedAreaId) ?? null
  const activeRemindersCount = Object.values(reminders).filter(Boolean).length

  return (
    <div>
      <DashboardHeader
        areas={areas}
        selectedArea={selectedArea}
        onSelectArea={selectArea}
        onAddArea={() => setModalOpen(true)}
      />

      {loading && <p>Loading…</p>}
      {error && <p className="error-text">{error}</p>}

      {!loading && !error && !selectedArea && (
        <div className="empty-state">
          <p className="empty-state-title">Add a place you want ParkAhead to watch</p>
          <p className="tagline">
            Search an address, set a radius, and you'll see upcoming events and parking risk for that area here.
          </p>
          <button type="button" className="button-primary" onClick={() => setModalOpen(true)}>
            Add your first area
          </button>
        </div>
      )}

      {!loading && !error && selectedArea && (
        <>
          <SummaryCards
            upcomingEventsCount={forecast?.summary.upcomingEventCount ?? 0}
            highRiskCount={forecast?.summary.highRiskEventCount ?? 0}
            activeRemindersCount={activeRemindersCount}
          />

          <h2 className="section-title" style={{ marginTop: 28 }}>
            Upcoming parking risk
          </h2>

          {forecastLoading && <p>Checking nearby events…</p>}
          {forecastError && (
            <p className="error-text">Could not load the risk forecast for this area: {forecastError}</p>
          )}
          {!forecastLoading && !forecastError && forecast && forecast.events.length === 0 && (
            <p>No upcoming events found near this area in the next 7 days.</p>
          )}

          {!forecastLoading && !forecastError && forecast && forecast.events.length > 0 && (
            <div className="risk-list">
              {forecast.events.map((event) => (
                <RiskCard
                  key={event.eventId}
                  event={event}
                  reminderOn={!!reminders[event.eventId]}
                  onToggleReminder={() => toggleReminder(event.eventId)}
                />
              ))}
            </div>
          )}
        </>
      )}

      {modalOpen && (
        <Modal title="Add a monitored area" onClose={() => setModalOpen(false)}>
          <CreateAreaForm onCreated={handleCreated} />
        </Modal>
      )}
    </div>
  )
}
