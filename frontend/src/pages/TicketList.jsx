import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getEditors } from '../api/auth.js'
import { assignEditor, changeStatus, getTickets } from '../api/tickets.js'
import { Role, useAuth } from '../authStore.js'
import DemoSwitches from '../components/DemoSwitches.jsx'
import TicketTable from '../components/TicketTable.jsx'
import { setItems, useTickets } from '../ticketsStore.js'

// Одно значение состояния экрана: loading | empty | data | error | unauthorized.
// Таблица, загрузка и ошибка никогда не показываются одновременно.
export default function TicketList() {
  const items = useTickets()
  const { user, checked } = useAuth()
  const [state, setState] = useState('loading')
  const [errorText, setErrorText] = useState('')
  const [actionError, setActionError] = useState('')
  const [editors, setEditors] = useState([])
  const [busyId, setBusyId] = useState(null)
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
        if (error.status === 401) {
          setState('unauthorized')
          return
        }
        setErrorText(error.message)
        setState('error')
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [attempt, user])

  // Список редакторов нужен только координатору — он один имеет на него право.
  useEffect(() => {
    if (user?.role !== Role.Coordinator) return undefined

    let cancelled = false
    getEditors()
      .then((list) => {
        if (!cancelled) setEditors(list)
      })
      .catch(() => {})

    return () => {
      cancelled = true
    }
  }, [user])

  const reload = useCallback(() => {
    setState('loading')
    setAttempt((value) => value + 1)
  }, [])

  async function runAction(item, action) {
    if (busyId) return
    setActionError('')
    setBusyId(item.id)
    try {
      await action()
      reload()
    } catch (error) {
      setActionError(error.message)
    } finally {
      setBusyId(null)
    }
  }

  const onAssign = (item, editorId) =>
    runAction(item, () => assignEditor(item.id, editorId))

  const onChangeStatus = (item, status) =>
    runAction(item, () => changeStatus(item.id, status))

  return (
    <main>
      <section aria-labelledby="list-heading">
        <h1 id="list-heading">Заявки на публикацию</h1>

        {actionError && (
          <p className="form-error" role="alert">
            {actionError}
          </p>
        )}

        {state === 'loading' && <p className="state">Загрузка…</p>}

        {state === 'unauthorized' && checked && (
          <p className="state">
            Чтобы увидеть заявки, <Link to="/login">войдите в систему</Link>.
          </p>
        )}

        {state === 'empty' && <p className="state">Записей пока нет</p>}

        {state === 'error' && (
          <div className="state state--error">
            <p>{errorText}</p>
            <button type="button" onClick={reload}>
              Повторить
            </button>
          </div>
        )}

        {state === 'data' && (
          <TicketTable
            items={items}
            user={user}
            editors={user?.role === Role.Coordinator ? editors : []}
            onAssign={onAssign}
            onChangeStatus={onChangeStatus}
            busyId={busyId}
          />
        )}

        <DemoSwitches onChange={reload} />
      </section>
    </main>
  )
}
