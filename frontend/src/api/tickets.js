import { getFlags, mockCreateTicket, mockGetTickets, setFlag } from './mockTickets.js'

// Один переключатель источника данных на весь клиент.
// В ЛР7, когда поднимется ASP.NET Core, достаточно поставить false.
const USE_MOCK = true

// Адрес сервера берётся из .env, а не пишется в страницах.
const baseUrl = import.meta.env.VITE_API_BASE_URL
const RESOURCE = '/api/publication-requests'

export class ApiError extends Error {
  constructor(message, status) {
    super(message)
    this.name = 'ApiError'
    // status === null означает «ответа не было»: сервер не запущен или нет сети.
    this.status = status
  }
}

function messageFor(status) {
  if (status === 400) return 'Проверьте поля'
  if (status === 404) return 'Запись не найдена'
  if (status === 409) return 'Действие сейчас недопустимо'
  if (status === null) return 'Нет связи с сервером'
  return 'Сервер не смог обработать запрос'
}

// Единая обёртка над fetch: возвращает ту же форму ответа, что и mock.
async function request(path, options) {
  let response
  try {
    response = await fetch(`${baseUrl}${path}`, options)
  } catch {
    // Сюда попадает только отсутствие ответа. Это не код 500.
    throw new ApiError(messageFor(null), null)
  }

  const data = response.status === 204 ? null : await response.json().catch(() => null)
  return { ok: response.ok, status: response.status, data }
}

function unwrap(response) {
  // Коды 400 и 500 приходят обычным ответом, а не исключением:
  // поэтому проверяем response.ok, а не полагаемся на catch.
  if (!response.ok) {
    const error = new ApiError(messageFor(response.status), response.status)
    error.fields = response.data?.fields ?? null
    throw error
  }
  return response.data
}

export async function getTickets() {
  const response = USE_MOCK ? await mockGetTickets() : await request(RESOURCE, { method: 'GET' })
  return unwrap(response)
}

export async function createTicket(body) {
  const response = USE_MOCK
    ? await mockCreateTicket(body)
    : await request(RESOURCE, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      })
  return unwrap(response)
}

// Учебные переключатели для демонстрации отказов; страницы не трогают mock напрямую.
export function setDemoFlag(name, value) {
  setFlag(name, value)
}

export function getDemoFlags() {
  return getFlags()
}

export const apiInfo = { useMock: USE_MOCK, baseUrl }
