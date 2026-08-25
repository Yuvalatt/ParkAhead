import type { RiskLevel } from '../types'

/**
 * DEMO DATA ONLY. The backend has no risk-calculation or event-ingestion feature yet
 * (see project scope) — everything in this file is a hardcoded placeholder so the
 * dashboard layout can be designed and reviewed before that feature exists. None of this
 * is fetched, computed, or persisted. Swap this module out once /risk-forecast is real.
 */
export interface MockRiskEvent {
  id: string
  title: string
  category: string
  when: string
  distanceLabel: string
  riskLevel: RiskLevel
  parkingPressurePercent: number
  reason: string
}

export const MOCK_RISK_EVENTS: MockRiskEvent[] = [
  {
    id: 'mock-1',
    title: 'City Summer Concert',
    category: 'Concert',
    when: 'Tonight · 20:30',
    distanceLabel: '850 m away',
    riskLevel: 'High',
    parkingPressurePercent: 78,
    reason: 'Large evening event close to your monitored area.',
  },
  {
    id: 'mock-2',
    title: 'Late Night Jazz Session',
    category: 'Live music',
    when: 'Tomorrow · 21:00',
    distanceLabel: '1.2 km away',
    riskLevel: 'Medium',
    parkingPressurePercent: 52,
    reason: 'Medium-sized venue with typical evening turnout.',
  },
  {
    id: 'mock-3',
    title: 'Downtown Food Market',
    category: 'Market',
    when: 'Saturday · 10:00',
    distanceLabel: '1.8 km away',
    riskLevel: 'Medium',
    parkingPressurePercent: 45,
    reason: 'Weekend foot traffic increases nearby demand.',
  },
  {
    id: 'mock-4',
    title: 'Community Fun Run',
    category: 'Sports',
    when: 'Sunday · 08:00',
    distanceLabel: '3.6 km away',
    riskLevel: 'Low',
    parkingPressurePercent: 18,
    reason: 'Small local event, limited impact expected.',
  },
  {
    id: 'mock-5',
    title: 'Neighborhood Art Walk',
    category: 'Community',
    when: 'Sunday · 17:00',
    distanceLabel: '4.1 km away',
    riskLevel: 'Low',
    parkingPressurePercent: 12,
    reason: 'Minor foot traffic, unlikely to affect parking.',
  },
]

export const MOCK_HIGH_RISK_COUNT = MOCK_RISK_EVENTS.filter((e) => e.riskLevel === 'High').length
