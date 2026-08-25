import type { AreaType } from '../types'

interface AreaTypeIconProps {
  type: AreaType
  size?: number
  className?: string
}

export function AreaTypeIcon({ type, size = 18, className }: AreaTypeIconProps) {
  const shared = {
    width: size,
    height: size,
    viewBox: '0 0 24 24',
    fill: 'none' as const,
    stroke: 'currentColor',
    strokeWidth: 1.8,
    strokeLinecap: 'round' as const,
    strokeLinejoin: 'round' as const,
    className,
    'aria-hidden': true,
  }

  switch (type) {
    case 'Home':
      return (
        <svg {...shared}>
          <path d="M3 10.5 12 4l9 6.5" />
          <path d="M5 9.5V20a1 1 0 0 0 1 1h4v-6h4v6h4a1 1 0 0 0 1-1V9.5" />
        </svg>
      )
    case 'Work':
      return (
        <svg {...shared}>
          <rect x="3" y="7" width="18" height="13" rx="2" />
          <path d="M8 7V5a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
          <path d="M3 12h18" />
        </svg>
      )
    case 'Other':
      return (
        <svg {...shared}>
          <path d="M12 21s-7-6.5-7-11.5A7 7 0 0 1 12 2a7 7 0 0 1 7 7.5C19 14.5 12 21 12 21Z" />
          <circle cx="12" cy="9.5" r="2.2" fill="currentColor" stroke="none" />
        </svg>
      )
  }
}
