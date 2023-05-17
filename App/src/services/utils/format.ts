export const capitalize = (text?: string) => `${text?.[0].toUpperCase()}${text?.substring(1)}`

export const shortDate = (date?: string) => new Date(date ?? "").toLocaleDateString()