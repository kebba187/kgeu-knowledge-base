import { ApiError, apiInfo } from './tickets.js'

// Вход всегда идёт на настоящий сервер: mock паролей не проверяет.
const baseUrl = apiInfo.baseUrl

async function send(path, options = {}) {
  let response
  try {
    response = await fetch(`${baseUrl}${path}`, {
      // credentials: cookie сессии ходит вместе с запросом на другой порт.
      credentials: 'include',
      headers: { 'Content-Type': 'application/json' },
      ...options,
    })
  } catch {
    throw new ApiError('Нет связи с сервером', null)
  }

  const data = response.status === 204 ? null : await response.json().catch(() => null)

  if (!response.ok) {
    const error = new ApiError(data?.message ?? 'Не удалось выполнить вход', response.status)
    throw error
  }

  return data
}

export function login(credentials) {
  return send('/api/auth/login', { method: 'POST', body: JSON.stringify(credentials) })
}

export function logout() {
  return send('/api/auth/logout', { method: 'POST' })
}

export function me() {
  return send('/api/auth/me', { method: 'GET' })
}

export function getEditors() {
  return send('/api/users?role=Editor', { method: 'GET' })
}
