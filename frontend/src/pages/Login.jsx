// Страница входа. Настоящая авторизация появится в ЛР11.
export default function Login() {
  return (
    <main>
      <section aria-labelledby="login-heading">
        <h1 id="login-heading">Вход</h1>
        <form onSubmit={(event) => event.preventDefault()}>
          <label htmlFor="login">Логин</label>
          <input id="login" name="login" type="text" autoComplete="username" required />

          <label htmlFor="password">Пароль</label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            required
          />

          <button type="submit">Войти</button>
        </form>
      </section>
    </main>
  )
}
