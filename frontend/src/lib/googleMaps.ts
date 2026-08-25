let loadPromise: Promise<void> | null = null

/**
 * Loads the Google Maps JavaScript API with the places library (no map is ever rendered)
 * exactly once per page, regardless of how many components request it.
 *
 * Deliberately omits `loading=async`: that mode requires Google's specific inline bootstrap
 * shim to make `importLibrary` safe to call before the main script finishes loading. Without
 * it, `onload` can fire before `google.maps.places` (or even `importLibrary` itself) is
 * populated. Loading synchronously with `libraries=places` in the URL guarantees the places
 * library is fully ready by the time `onload` fires — simpler and reliable for this app's needs.
 */
export function loadGoogleMapsScript(apiKey: string): Promise<void> {
  if (typeof google !== 'undefined' && google.maps?.places) {
    return Promise.resolve()
  }

  if (!loadPromise) {
    loadPromise = new Promise((resolve, reject) => {
      const script = document.createElement('script')
      script.src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&libraries=places`
      script.async = true
      script.onload = () => resolve()
      script.onerror = () => {
        loadPromise = null
        reject(new Error('Failed to load the Google Maps script.'))
      }
      document.head.appendChild(script)
    })
  }

  return loadPromise
}
