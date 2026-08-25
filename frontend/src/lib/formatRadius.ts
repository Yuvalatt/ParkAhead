export function formatRadius(meters: number): string {
  if (meters >= 1000) {
    const km = meters / 1000
    return `${km % 1 === 0 ? km : km.toFixed(1)} km`
  }
  return `${meters} m`
}
