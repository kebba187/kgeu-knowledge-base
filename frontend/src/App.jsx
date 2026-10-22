import AppHeader from './components/AppHeader.jsx'
import AppRoutes from './router.jsx'
import './App.css'

// Шапка стоит СНАРУЖИ маршрутов: она видна на всех страницах и не копируется в них.
export default function App() {
  return (
    <>
      <AppHeader />
      <AppRoutes />
    </>
  )
}
