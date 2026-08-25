export type RiskLevel = 'Low' | 'Medium' | 'High'

export type AreaType = 'Home' | 'Work' | 'Other'

export type EventCategory = 'Concert' | 'Sports' | 'Festival' | 'Conference' | 'Other'

export interface MonitoredArea {
  id: string
  name: string
  areaType: AreaType
  address: string
  latitude: number
  longitude: number
  radiusMeters: number
}

export interface EventRisk {
  eventId: string
  title: string
  venueName: string | null
  startTime: string
  category: EventCategory
  distanceKm: number
  estimatedAttendance: number | null
  riskScore: number
  riskLevel: RiskLevel
  reasons: string[]
}

export interface RiskForecastSummary {
  upcomingEventCount: number
  highRiskEventCount: number
}

export interface RiskForecast {
  monitoredArea: MonitoredArea
  generatedAt: string
  summary: RiskForecastSummary
  events: EventRisk[]
}
