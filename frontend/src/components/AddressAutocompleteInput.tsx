import { useEffect, useRef, useState } from 'react'
import { loadGoogleMapsScript } from '../lib/googleMaps'

export interface SelectedPlace {
  address: string
  latitude: number
  longitude: number
}

interface AddressAutocompleteInputProps {
  value: string
  onTextChange: (text: string) => void
  onPlaceSelected: (place: SelectedPlace) => void
  disabled?: boolean
}

const API_KEY = import.meta.env.VITE_GOOGLE_MAPS_API_KEY

export function AddressAutocompleteInput({
  value,
  onTextChange,
  onPlaceSelected,
  disabled,
}: AddressAutocompleteInputProps) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [scriptError, setScriptError] = useState<string | null>(null)

  useEffect(() => {
    if (!API_KEY) {
      return
    }

    let autocomplete: google.maps.places.Autocomplete | undefined
    let cancelled = false

    loadGoogleMapsScript(API_KEY)
      .then(() => {
        if (cancelled || !inputRef.current) return

        autocomplete = new google.maps.places.Autocomplete(inputRef.current, {
          // Israel-only, per product scope.
          componentRestrictions: { country: 'il' },
          fields: ['formatted_address', 'geometry'],
        })

        autocomplete.addListener('place_changed', () => {
          const place = autocomplete!.getPlace()
          const location = place.geometry?.location

          if (!location || !place.formatted_address) {
            return
          }

          onPlaceSelected({
            address: place.formatted_address,
            latitude: location.lat(),
            longitude: location.lng(),
          })
        })
      })
      .catch((err: Error) => setScriptError(err.message))

    return () => {
      cancelled = true
      if (autocomplete) {
        google.maps.event.clearInstanceListeners(autocomplete)
      }
    }
    // Runs once: the Autocomplete instance is bound directly to the input DOM node, not to React state.
  }, [])

  if (!API_KEY) {
    return (
      <div>
        <input type="text" disabled placeholder="Address autocomplete unavailable" className="input" />
        <p className="error-text">
          Google Maps API key is not configured. Set VITE_GOOGLE_MAPS_API_KEY in your .env file to enable address
          search.
        </p>
      </div>
    )
  }

  return (
    <div>
      <input
        ref={inputRef}
        type="text"
        value={value}
        onChange={(e) => onTextChange(e.target.value)}
        placeholder="Start typing an address…"
        disabled={disabled}
        className="input"
      />
      {scriptError && <p className="error-text">{scriptError}</p>}
    </div>
  )
}
