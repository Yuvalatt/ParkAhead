interface SummaryCardsProps {
  upcomingEventsCount: number
  highRiskCount: number
  activeRemindersCount: number
}

export function SummaryCards({ upcomingEventsCount, highRiskCount, activeRemindersCount }: SummaryCardsProps) {
  return (
    <div>
      <h2 className="section-title">Summary</h2>
      <div className="summary-grid">
        <div className="summary-card">
          <div className="summary-value">{upcomingEventsCount}</div>
          <div className="summary-label">upcoming events</div>
        </div>
        <div className="summary-card">
          <div className="summary-value summary-value--high">{highRiskCount}</div>
          <div className="summary-label">high-risk nearby</div>
        </div>
        <div className="summary-card">
          <div className="summary-value">{activeRemindersCount}</div>
          <div className="summary-label">reminders on</div>
          <div className="demo-tag">Demo — not yet wired up</div>
        </div>
      </div>
    </div>
  )
}
