import { useSyncExternalStore } from 'react'
import { login as apiLogin, logout as apiLogout, me as apiMe } from './api/auth.js'

// Кто вошёл. Храним только то, что вернул сервер: ни пароля, ни токена.
// Сессия живёт в cookie, которую JavaScript не читает (HttpOnly).
let state = { user: null, checked: false }

const listeners = new Set()

function emit() {
  listeners.forEach((listener) => listener())
}

function subscribe(listener) {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

function getState() {
  return state
}

function setState(next) {
  state = next
  emit()
}

export function useAuth() {
  return useSyncExternalStore(subscribe, getState)
}

/// Проверить при старте, есть ли живая сессия.
export async function refreshSession() {
  try {
    const user = await apiMe()
    setState({ user, checked: true })
  } catch {
    setState({ user: null, checked: true })
  }
}

export async function signIn(credentials) {
  const user = await apiLogin(credentials)
  setState({ user, checked: true })
  return user
}

export async function signOut() {
  try {
    await apiLogout()
  } finally {
    setState({ user: null, checked: true })
  }
}

export const Role = {
  Author: 'Author',
  Coordinator: 'Coordinator',
  Editor: 'Editor',
}

export const roleLabels = {
  Author: 'автор',
  Coordinator: 'координатор',
  Editor: 'редактор',
}
