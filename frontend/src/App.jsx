import { useEffect, useRef, useState } from 'react'
import './App.css'

// Справочник подразделений. Пока в коде; с ЛР6 — из GET /api/departments.
const departments = [
  { id: 1, name: 'Учебный отдел' },
  { id: 2, name: 'Деканат' },
  { id: 3, name: 'Студенческий городок' },
  { id: 4, name: 'ИТ-служба' },
]

// Коды статусов — как в API и БД (docs/api-contract.md). Подписи на экране — свои.
const statusLabels = {
  New: 'Поступила',
  InProgress: 'На проверке',
  Closed: 'Опубликовано',
  Cancelled: 'Отклонено',
}

// Тестовые заявки на публикацию. Имена полей — как в docs/data-model.md.
const items = [
  {
    id: 1,
    number: 'БЗ-2026-0001',
    title: 'Порядок оформления академического отпуска',
    departmentId: 1,
    status: 'New',
  },
  {
    id: 2,
    number: 'БЗ-2026-0002',
    title: 'Как получить доступ к ЭИОС',
    departmentId: 4,
    status: 'InProgress',
  },
  {
    id: 3,
    number: 'БЗ-2026-0003',
    title: 'Заселение в общежитие',
    departmentId: 3,
    status: 'Closed',
  },
]

function departmentName(id) {
  return departments.find((d) => d.id === id)?.name ?? '—'
}

export default function App() {
  // Какой экран показан: 'list' | 'new' | 'login'.
  // Настоящие адреса (/requests, /requests/new, /login) появятся в ЛР4.
  const [screen, setScreen] = useState('list')

  // При переходе на экран создания фокус ставится на первое поле.
  const titleInput = useRef(null)
  useEffect(() => {
    if (screen === 'new') titleInput.current?.focus()
  }, [screen])

  return (
    <>
      <header className="app-header">
        <button type="button" className="brand" onClick={() => setScreen('list')}>
        База знаний КГЭУ
        </button>
        <nav aria-label="Экраны">
          <button type="button" aria-pressed={screen === 'list'} onClick={() => setScreen('list')}>
            Заявки
          </button>
          <button type="button" aria-pressed={screen === 'new'} onClick={() => setScreen('new')}>
            Новая заявка
          </button>
          <button type="button" aria-pressed={screen === 'login'} onClick={() => setScreen('login')}>
            Вход
          </button>
        </nav>
      </header>

      <main>
        {/* Экран 1. Список заявок */}
        {screen === 'list' && (
          <section aria-labelledby="list-heading">
            <h1 id="list-heading">Заявки на публикацию</h1>
            <div className="table-wrap">
              <table>
                <caption>Тестовые данные. Сервер подключится в ЛР6.</caption>
                <thead>
                  <tr>
                    <th scope="col">Номер</th>
                    <th scope="col">Заголовок</th>
                    <th scope="col">Подразделение (departmentId)</th>
                    <th scope="col">Статус</th>
                  </tr>
                </thead>
                <tbody>
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td className="nowrap">{item.number}</td>
                      <td>{item.title}</td>
                      <td>
                        {item.departmentId} · {departmentName(item.departmentId)}
                      </td>
                      <td>
                        <span className={`status status--${item.status}`}>
                          {statusLabels[item.status]}
                        </span>
                        <code>{item.status}</code>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </section>
        )}

        {/* Экран 2. Создание заявки */}
        {screen === 'new' && (
          <section aria-labelledby="new-heading">
            <h1 id="new-heading">Новая заявка на публикацию</h1>
            <form onSubmit={(event) => event.preventDefault()}>
              <label htmlFor="title">Заголовок, 5–80 символов</label>
              <input
                id="title"
                ref={titleInput}
                name="title"
                type="text"
                minLength={5}
                maxLength={80}
                required
              />

              <label htmlFor="departmentId">Подразделение (номер из справочника)</label>
              <input
                id="departmentId"
                name="departmentId"
                type="number"
                min={1}
                required
                aria-describedby="departmentId-hint"
              />
              <p id="departmentId-hint" className="hint">
                {departments.map((d) => (
                  <span key={d.id}>
                    {d.id} — {d.name}
                  </span>
                ))}
              </p>

              <label htmlFor="description">Описание, 10–500 символов</label>
              <textarea
                id="description"
                name="description"
                rows={5}
                minLength={10}
                maxLength={500}
                required
              />

              <button type="submit">Создать</button>
            </form>
          </section>
        )}

        {/* Экран 3. Вход */}
        {screen === 'login' && (
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
        )}
      </main>
    </>
  )
}
