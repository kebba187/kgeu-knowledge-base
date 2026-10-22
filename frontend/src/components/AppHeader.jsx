import { NavLink } from 'react-router-dom'

// Шапка одна на всё приложение. Её рисует App.jsx снаружи маршрутов.
export default function AppHeader() {
  return (
    <header className="app-header">
      <NavLink to="/" className="brand">
        База знаний КГЭУ
      </NavLink>
      <nav aria-label="Разделы">
        <NavLink to="/">Заявки</NavLink>
        <NavLink to="/new">Новая заявка</NavLink>
        <NavLink to="/login">Вход</NavLink>
      </nav>
    </header>
  )
}
