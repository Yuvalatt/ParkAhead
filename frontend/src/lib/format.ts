export function formatDistance(distanceKm: number): string {
  if (distanceKm < 1) {
    return `${Math.round(distanceKm * 1000)} m away`
  }
  return `${distanceKm.toFixed(1)} km away`
}

export function formatEventTime(startTime: string): string {
  return new Date(startTime).toLocaleString(undefined, {
    weekday: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })
}
