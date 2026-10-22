import { useSyncExternalStore } from 'react'

// Единственный общий массив заявок на весь клиент.
// Страницы и компоненты импортируют его только отсюда.
let items = [
  {
    id: 1,
    number: 'БЗ-2026-0001',
    title: 'Порядок оформления академического отпуска',
    departmentId: 1,
    description: 'Какие документы нужны, куда подавать и в какой срок рассматривают.',
    status: 'New',
  },
  {
    id: 2,
    number: 'БЗ-2026-0002',
    title: 'Как получить доступ к ЭИОС',
    departmentId: 4,
    description: 'Где взять логин, что делать при утере пароля.',
    status: 'InProgress',
  },
  {
    id: 3,
    number: 'БЗ-2026-0003',
    title: 'Заселение в общежитие',
    departmentId: 3,
    description: 'Список документов и порядок заселения первокурсников.',
    status: 'Closed',
  },
]

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
