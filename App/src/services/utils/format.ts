export const capitalize = (text?: string) => `${text?.[0].toUpperCase()}${text?.substring(1)}`

export const shortDate = (date?: string) => new Date(date ?? "").toLocaleDateString()

export const formatErrors = (errorObject?: { [key: string]: string }) => Object.entries(errorObject ?? {}).map(([k, v]) => `${k}: ${v}`).join("\\n")
