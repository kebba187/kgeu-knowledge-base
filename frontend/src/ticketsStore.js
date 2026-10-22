import { useSyncExternalStore } from 'react'

// Единственный общий массив заявок на весь клиент.
// Страницы и компоненты импортируют его только отсюда.
// Список приходит из API (ЛР6), поэтому стартуем с пустого массива.
let items = []

// Подписчики — это компоненты, которые должны перерисоваться при изменении массива.
const listeners = new Set()

function emit() {
  listeners.forEach((listener) => listener())
}

function subscribe(listener) {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

function getItems() {
  return items
}

// Хук для компонентов: вернуть текущий массив и перерисоваться при его замене.
export function useTickets() {
  return useSyncExternalStore(subscribe, getItems)
}

export function setItems(nextItems) {
  items = nextItems
  emit()
}

// Добавить в список заявку, которую вернул сервер (или mock).
// id, number и status приходят из ответа — клиент их не придумывает.
export function appendTicket(ticket) {
  items = [...items, ticket]
  emit()
}
