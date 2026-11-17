import { useEffect } from 'react'
import AppHeader from './components/AppHeader.jsx'
import AppRoutes from './router.jsx'
import { refreshSession } from './authStore.js'
import './App.css'

// Шапка стоит СНАРУЖИ маршрутов: она видна на всех страницах и не копируется в них.
export default function App() {
  // При старте спрашиваем сервер, жива ли сессия из cookie.
  useEffect(() => {
    refreshSession()
  }, [])

  return (
    <>
      <AppHeader />
      <AppRoutes />
    </>
  )
}
