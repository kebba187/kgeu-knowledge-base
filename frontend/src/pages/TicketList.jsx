import { useEffect, useState } from 'react'
import { getTickets } from '../api/tickets.js'
import DemoSwitches from '../components/DemoSwitches.jsx'
import TicketTable from '../components/TicketTable.jsx'
import { setItems, useTickets } from '../ticketsStore.js'

// Одно значение состояния экрана: loading | empty | data | error.
// Таблица, загрузка и ошибка никогда не показываются одновременно.
export default function TicketList() {
  const items = useTickets()
  const [state, setState] = useState('loading')
  const [errorText, setErrorText] = useState('')
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    // cancelled защищает от записи результата после ухода со страницы.
    let cancelled = false

    async function load() {
      try {
        const data = await getTickets()
        if (cancelled) return
        setItems(data)
        setState(data.length === 0 ? 'empty' : 'data')
      } catch (error) {
        if (cancelled) return
        setErrorText(error.message)
        setState('error')
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [attempt])

  function retry() {
    setState('loading')
    setAttempt((value) => value + 1)
  }

  return (
    <main>
      <section aria-labelledby="list-heading">
        <h1 id="list-heading">Заявки на публикацию</h1>

        {state === 'loading' && <p className="state">Загрузка…</p>}

        {state === 'empty' && <p className="state">Записей пока нет</p>}

        {state === 'error' && (
          <div className="state state--error">
            <p>{errorText}</p>
            <button type="button" onClick={retry}>
              Повторить
            </button>
          </div>
        )}

        {state === 'data' && <TicketTable items={items} />}

        <DemoSwitches onChange={retry} />
      </section>
    </main>
  )
}
