import type { Config } from 'tailwindcss'

const config: Config = {
  content: [
    './app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      transitionTimingFunction: {
        'in-bounce': 'cubic-bezier(.47,1.64,.41,.8)',
      },
      boxShadow: {
        'inner-md': 'inset 0 4px 6px -1px rgba(0, 0, 0, 0.1), inset 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
      },
      spacing: {
        'header-h': '5rem',
      },
    },
    plugins: [],
  }
}

export default config
