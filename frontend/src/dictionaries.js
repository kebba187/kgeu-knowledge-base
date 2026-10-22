// Справочник подразделений. Пока в коде; на сервере это таблица departments.
export const departments = [
  { id: 1, name: 'Учебный отдел' },
  { id: 2, name: 'Деканат' },
  { id: 3, name: 'Студенческий городок' },
  { id: 4, name: 'ИТ-служба' },
]

export function departmentName(id) {
  return departments.find((d) => d.id === id)?.name ?? '—'
}

// Коды статусов — как в API и БД (docs/api-contract.md). Подписи на экране — свои.
export const statusLabels = {
  New: 'Поступила',
  InProgress: 'На проверке',
  Closed: 'Опубликовано',
  Cancelled: 'Отклонено',
}
