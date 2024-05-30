function convertUTCtoLocal(utcString: string): string {
  const utcDate = new Date(utcString + "Z") // Append 'Z' to indicate UTC time
  const now = new Date()

  const locales = navigator.languages || [] // Get user's preferred locales

  // Determine the date format based on recency
  let options: Intl.DateTimeFormatOptions

  if (now.getUTCDay() === utcDate.getUTCDay()) {
    options = { hour: "numeric", minute: "numeric" }
  } else if (now.getDay() - utcDate.getDay() < 7) {
    options = { weekday: "short" }
  } else if (now.getUTCFullYear() === utcDate.getUTCFullYear()) {
    options = { month: "short", day: "numeric" }
  } else {
    options = { year: "numeric", month: "short", day: "numeric" }
  }

  const formatter = new Intl.DateTimeFormat(locales, options)
  return formatter.format(utcDate)
}

export { convertUTCtoLocal }
