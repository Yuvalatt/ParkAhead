import type { MonitoredArea, RiskForecastDay } from '../types'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...init,
  })

  if (!response.ok) {
    throw new Error(`Request to ${path} failed with status ${response.status}`)
  }

  return response.json() as Promise<T>
}

export const api = {
  getMonitoredAreas: () => request<MonitoredArea[]>('/api/monitored-areas'),

  createMonitoredArea: (area: Omit<MonitoredArea, 'id'>) =>
    request<MonitoredArea>('/api/monitored-areas', {
      method: 'POST',
      body: JSON.stringify(area),
    }),

  getRiskForecast: (areaId: string, days = 7) =>
    request<RiskForecastDay[]>(`/api/monitored-areas/${areaId}/risk-forecast?days=${days}`),
}
