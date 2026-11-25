import { ApiError, apiInfo } from './tickets.js'

// Справочник подразделений приходит с сервера: таблица departments.
// Своей копии списка у клиента больше нет (D-02 в docs/known-defects.md).
export async function getDepartments() {
  let response
  try {
    response = await fetch(`${apiInfo.baseUrl}/api/departments`, {
      credentials: 'include',
      headers: { 'Content-Type': 'application/json' },
      method: 'GET',
    })
  } catch {
    throw new ApiError('Нет связи с сервером', null)
  }

  const data = await response.json().catch(() => null)

  if (!response.ok) {
    throw new ApiError(data?.message ?? 'Не удалось загрузить справочник', response.status)
  }

  return data ?? []
}
