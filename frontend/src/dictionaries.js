// Коды статусов — как в API и БД (docs/api-contract.md). Подписи на экране — свои.
// Справочник подразделений здесь больше не хранится: он приходит
// из GET /api/departments, иначе клиент и база расходятся (D-02).
export const statusLabels = {
  New: 'Поступила',
  InProgress: 'На проверке',
  Closed: 'Опубликовано',
  Cancelled: 'Отклонено',
}
