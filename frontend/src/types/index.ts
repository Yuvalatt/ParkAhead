export type RiskLevel = 'Low' | 'Medium' | 'High'

export interface MonitoredArea {
  id: string
  name: string
  address: string
  latitude: number
  longitude: number
  radiusMeters: number
}

export interface ContributingEvent {
  eventId: string
  title: string
  venueName: string | null
  distanceMeters: number
  startDateTime: string
  reason: string
}

export interface RiskForecastDay {
  date: string
  riskLevel: RiskLevel
  riskScore: number
  contributingEvents: ContributingEvent[]
}
