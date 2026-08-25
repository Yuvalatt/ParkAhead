import { useEffect, useState } from 'react'
import { api } from '../api/client'
import { CreateAreaForm } from '../components/CreateAreaForm'
import { DashboardHeader } from '../components/DashboardHeader'
import { Modal } from '../components/Modal'
import { RiskCard } from '../components/RiskCard'
import { SummaryCards } from '../components/SummaryCards'
import { MOCK_HIGH_RISK_COUNT, MOCK_RISK_EVENTS } from '../lib/mockRiskData'
import type { MonitoredArea } from '../types'

const SELECTED_AREA_STORAGE_KEY = 'parkahead:selectedAreaId'

// A couple of reminders start "on" so the dashboard doesn't look inert on first load —
// this is UI-only state (see mockRiskData.ts), not backed by any notification system yet.
const DEFAULT_REMINDERS: Record<string, boolean> = { 'mock-1': true, 'mock-4': true }

export function AreasPage() {
  const [areas, setAreas] = useState<MonitoredArea[]>([])
  const [selectedAreaId, setSelectedAreaId] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [modalOpen, setModalOpen] = useState(false)
  const [reminders, setReminders] = useState(DEFAULT_REMINDERS)

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
            upcomingEventsCount={MOCK_RISK_EVENTS.length}
            highRiskCount={MOCK_HIGH_RISK_COUNT}
            activeRemindersCount={activeRemindersCount}
          />

          <div className="section-title-row" style={{ marginTop: 28 }}>
            <h2 className="section-title">Upcoming parking risk</h2>
            <span className="demo-pill">Demo data</span>
          </div>

          <div className="risk-list">
            {MOCK_RISK_EVENTS.map((event) => (
              <RiskCard
                key={event.id}
                event={event}
                reminderOn={!!reminders[event.id]}
                onToggleReminder={() => toggleReminder(event.id)}
              />
            ))}
          </div>
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
