import { formatRadius } from '../lib/formatRadius'
import type { MonitoredArea } from '../types'
import { AreaTypeIcon } from './AreaTypeIcon'

interface DashboardHeaderProps {
  areas: MonitoredArea[]
  selectedArea: MonitoredArea | null
  onSelectArea: (id: string) => void
  onAddArea: () => void
}

export function DashboardHeader({ areas, selectedArea, onSelectArea, onAddArea }: DashboardHeaderProps) {
  return (
    <header className="app-header">
      <div className="brand-row">
        <span className="brand-icon">P</span>
        <div>
          <h1 className="brand-name">ParkAhead</h1>
          <p className="tagline">Know before you drive whether parking nearby will be difficult.</p>
        </div>
      </div>

      <div className="tracking-row">
        <p className="tracking-line">
          {selectedArea ? (
            <>
              Tracking: <AreaTypeIcon type={selectedArea.areaType} size={15} className="tracking-icon" />{' '}
              <strong>{selectedArea.name}</strong> · {selectedArea.address} ·{' '}
              {formatRadius(selectedArea.radiusMeters)} radius
            </>
          ) : (
            'No monitored area yet'
          )}
        </p>

        <div className="header-actions">
          {areas.length > 1 && (
            <div className="area-select-wrap">
              {selectedArea && <AreaTypeIcon type={selectedArea.areaType} size={16} />}
              <select
                className="area-select"
                aria-label="Change monitored area"
                value={selectedArea?.id ?? ''}
                onChange={(e) => onSelectArea(e.target.value)}
              >
                {areas.map((area) => (
                  <option key={area.id} value={area.id}>
                    {area.name}
                  </option>
                ))}
              </select>
            </div>
          )}
          <button type="button" className="btn-secondary" onClick={onAddArea}>
            {selectedArea ? 'Add area' : 'Add your first area'}
          </button>
        </div>
      </div>
    </header>
  )
}
