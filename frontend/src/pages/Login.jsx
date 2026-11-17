import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { roleLabels, signIn, useAuth } from '../authStore.js'

export default function Login() {
  const { user } = useAuth()
  const [form, setForm] = useState({ login: '', password: '' })
  const [error, setError] = useState('')
  const [isSubmitting, setSubmitting] = useState(false)
  const navigate = useNavigate()

  const loginInput = useRef(null)
  useEffect(() => {
    loginInput.current?.focus()
  }, [])

  async function onSubmit(event) {
    event.preventDefault()
    if (isSubmitting) return

    setError('')
    setSubmitting(true)
    try {
      await signIn({ login: form.login.trim(), password: form.password })
      navigate('/')
    } catch (apiError) {
      setError(apiError.message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <main>
      <section aria-labelledby="login-heading">
        <h1 id="login-heading">Вход</h1>

        {user && (
          <p className="state">
            Вы вошли как {user.fullName} ({roleLabels[user.role] ?? user.role}).
          </p>
        )}

        <form onSubmit={onSubmit} noValidate>
          {error && (
            <p className="form-error" role="alert">
              {error}
            </p>
          )}

          <label htmlFor="login">Логин</label>
          <input
            id="login"
            ref={loginInput}
            name="login"
            type="text"
            autoComplete="username"
            value={form.login}
            onChange={(event) => setForm({ ...form, login: event.target.value })}
          />

          <label htmlFor="password">Пароль</label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            value={form.password}
            onChange={(event) => setForm({ ...form, password: event.target.value })}
          />

          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Входим…' : 'Войти'}
          </button>
        </form>
      </section>
    </main>
  )
}
