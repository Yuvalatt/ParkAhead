import { useState, type FormEvent } from 'react'
import { api } from '../api/client'
import type { AreaType, MonitoredArea } from '../types'
import { AddressAutocompleteInput, type SelectedPlace } from './AddressAutocompleteInput'
import { AreaTypeIcon } from './AreaTypeIcon'

interface CreateAreaFormProps {
  onCreated: (area: MonitoredArea) => void
}

const DEFAULT_RADIUS_METERS = 1000
const AREA_TYPES: AreaType[] = ['Home', 'Work', 'Other']

export function CreateAreaForm({ onCreated }: CreateAreaFormProps) {
  const [name, setName] = useState('')
  const [areaType, setAreaType] = useState<AreaType>('Home')
  const [addressText, setAddressText] = useState('')
  const [selectedPlace, setSelectedPlace] = useState<SelectedPlace | null>(null)
  const [radiusMeters, setRadiusMeters] = useState(DEFAULT_RADIUS_METERS)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const canSave = name.trim() !== '' && selectedPlace !== null && radiusMeters > 0 && !submitting

  function handleAddressTextChange(text: string) {
    setAddressText(text)
    // Any manual edit invalidates the previously selected place until a new suggestion is chosen.
    setSelectedPlace(null)
  }

  function handlePlaceSelected(place: SelectedPlace) {
    setAddressText(place.address)
    setSelectedPlace(place)
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    if (!selectedPlace) return

    setSubmitting(true)
    setError(null)

    try {
      const created = await api.createMonitoredArea({
        name: name.trim(),
        areaType,
        address: selectedPlace.address,
        latitude: selectedPlace.latitude,
        longitude: selectedPlace.longitude,
        radiusMeters,
      })

      onCreated(created)
      setName('')
      setAreaType('Home')
      setAddressText('')
      setSelectedPlace(null)
      setRadiusMeters(DEFAULT_RADIUS_METERS)
    } catch {
      setError('Could not save this monitored area. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="area-form">
      <label className="field">
        Name
        <input
          className="input"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="e.g. Home"
          disabled={submitting}
        />
      </label>

      <div className="field">
        Type
        <div className="segmented-control" role="radiogroup" aria-label="Area type">
          {AREA_TYPES.map((option) => (
            <button
              key={option}
              type="button"
              role="radio"
              aria-checked={areaType === option}
              className={`segmented-option${areaType === option ? ' segmented-option--active' : ''}`}
              onClick={() => setAreaType(option)}
              disabled={submitting}
            >
              <AreaTypeIcon type={option} size={16} />
              {option}
            </button>
          ))}
        </div>
      </div>

      <label className="field">
        Address
        <AddressAutocompleteInput
          value={addressText}
          onTextChange={handleAddressTextChange}
          onPlaceSelected={handlePlaceSelected}
          disabled={submitting}
        />
      </label>

      <label className="field">
        Radius (meters)
        <input
          className="input"
          type="number"
          min={1}
          max={20000}
          step={100}
          value={radiusMeters}
          onChange={(e) => setRadiusMeters(Number(e.target.value))}
          disabled={submitting}
        />
      </label>

      {error && <p className="error-text">{error}</p>}

      <button type="submit" className="button-primary" disabled={!canSave}>
        {submitting ? 'Saving…' : 'Save monitored area'}
      </button>
    </form>
  )
}
