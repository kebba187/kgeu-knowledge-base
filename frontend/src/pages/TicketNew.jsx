import { useEffect, useRef } from 'react'
import { departments } from '../dictionaries.js'

// Пока только разметка формы: запись в общий массив появится в ЛР5.
export default function TicketNew() {
  const titleInput = useRef(null)
  useEffect(() => {
    titleInput.current?.focus()
  }, [])

  return (
    <main>
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
    </main>
  )
}
