import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { departments } from '../dictionaries.js'
import { addItem } from '../ticketsStore.js'

const emptyForm = { title: '', departmentId: '', description: '' }

// Проверки курса: тема 5–80 после trim, departmentId — целое > 0 из справочника,
// описание 10–500. Клиентская проверка помогает человеку, серверная будет в ЛР8.
function validate(form) {
  const errors = {}
  const title = form.title.trim()

  if (title.length < 5 || title.length > 80) {
    errors.title = 'Заголовок: от 5 до 80 символов'
  }
  if (!Number.isInteger(form.departmentId) || form.departmentId <= 0) {
    errors.departmentId = 'Подразделение: целое число больше нуля'
  } else if (!departments.some((d) => d.id === form.departmentId)) {
    errors.departmentId = 'Такого подразделения нет в справочнике'
  }
  if (form.description.trim().length < 10 || form.description.trim().length > 500) {
    errors.description = 'Описание: от 10 до 500 символов'
  }
  return errors
}

export default function TicketNew() {
  const [form, setForm] = useState(emptyForm)
  const [errors, setErrors] = useState({})
  const navigate = useNavigate()

  const titleInput = useRef(null)
  useEffect(() => {
    titleInput.current?.focus()
  }, [])

  function updateField(field, value) {
    setForm((previous) => ({ ...previous, [field]: value }))
  }

  function onCreate(event) {
    // Без preventDefault браузер перезагрузил бы страницу и состояние пропало бы.
    event.preventDefault()

    const found = validate(form)
    setErrors(found)
    if (Object.keys(found).length > 0) return

    addItem({
      title: form.title.trim(),
      departmentId: form.departmentId,
      description: form.description.trim(),
    })

    setForm(emptyForm)
    setErrors({})
    navigate('/')
  }

  return (
    <main>
      <section aria-labelledby="new-heading">
        <h1 id="new-heading">Новая заявка на публикацию</h1>
        <form onSubmit={onCreate} noValidate>
          <label htmlFor="title">Заголовок, 5–80 символов</label>
          <input
            id="title"
            ref={titleInput}
            name="title"
            type="text"
            value={form.title}
            onChange={(event) => updateField('title', event.target.value)}
            aria-invalid={Boolean(errors.title)}
            aria-describedby={errors.title ? 'title-error' : undefined}
          />
          {errors.title && (
            <p id="title-error" className="field-error">
              {errors.title}
            </p>
          )}

          <label htmlFor="departmentId">Подразделение (номер из справочника)</label>
          <input
            id="departmentId"
            name="departmentId"
            type="number"
            min={1}
            value={form.departmentId}
            onChange={(event) => {
              // Поле number отдаёт строку: приводим к числу, как v-model.number во Vue.
              const raw = event.target.value
              updateField('departmentId', raw === '' ? '' : Number(raw))
            }}
            aria-invalid={Boolean(errors.departmentId)}
            aria-describedby={
              errors.departmentId ? 'departmentId-error departmentId-hint' : 'departmentId-hint'
            }
          />
          {errors.departmentId && (
            <p id="departmentId-error" className="field-error">
              {errors.departmentId}
            </p>
          )}
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
            value={form.description}
            onChange={(event) => updateField('description', event.target.value)}
            aria-invalid={Boolean(errors.description)}
            aria-describedby={errors.description ? 'description-error' : undefined}
          />
          {errors.description && (
            <p id="description-error" className="field-error">
              {errors.description}
            </p>
          )}

          <button type="submit">Создать</button>
        </form>
      </section>
    </main>
  )
}
