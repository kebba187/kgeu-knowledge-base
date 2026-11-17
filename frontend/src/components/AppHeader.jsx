import { NavLink, useNavigate } from 'react-router-dom'
import { roleLabels, signOut, useAuth } from '../authStore.js'
import logo from '../assets/kgeu-logo.jfif'

export default function AppHeader() {
  const { user } = useAuth()
  const navigate = useNavigate()

  async function onSignOut() {
    await signOut()
    navigate('/login')
  }

  return (
    <header className="app-header">
      <NavLink to="/" className="brand" aria-label="На главную">
        <img src={logo} alt="КГЭУ" className="brand-logo" />
        <span>База знаний КГЭУ</span>
      </NavLink>

      <nav aria-label="Разделы">
        <NavLink to="/">Заявки</NavLink>
        {user?.role === 'Author' && <NavLink to="/new">Новая заявка</NavLink>}
        {user ? (
          <span className="current-user">
            {user.fullName} · {roleLabels[user.role] ?? user.role}
            <button type="button" onClick={onSignOut}>
              Выйти
            </button>
          </span>
        ) : (
          <NavLink to="/login">Вход</NavLink>
        )}
      </nav>
    </header>
  )
}