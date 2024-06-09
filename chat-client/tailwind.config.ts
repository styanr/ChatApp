import type { Config } from "tailwindcss"

export default {
  content: ["./src/**/*.{html,js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        "ca-dark": "#131313",
        "ca-gray": "#2e333d",
        "ca-light-gray": "#444b5a",
        "ca-dark-gray": "#202329",
        "ca-blue": "#6b8afd",
        "ca-light-blue": "#9db2fe",
        "ca-dark-blue": "#3962fc",
        "ca-white": "#ffffff",
      },
    },
    fontFamily: {
      sans: ["Archivo", "sans-serif"],
    },
  },
  plugins: [],
} satisfies Config
