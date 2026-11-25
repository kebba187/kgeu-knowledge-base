import PropTypes from 'prop-types'
import { statusLabels } from '../dictionaries.js'
import { Role } from '../authStore.js'

// Таблица только рисует то, что ей передали через props.
// Своего массива у неё нет и копию входного массива она не делает.
export default function TicketTable({
  items,
  user,
  editors = [],
  onAssign,
  onChangeStatus,
  busyId,
}) {
  const showActions = Boolean(user) && (user.role === Role.Coordinator || user.role === Role.Editor)

  return (
    <div className="table-wrap">
      <table>
        <caption>Заявки на публикацию материалов базы знаний</caption>
        <thead>
          <tr>
            <th scope="col">Номер</th>
            <th scope="col">Заголовок</th>
            <th scope="col">Подразделение</th>
            <th scope="col">Статус</th>
            <th scope="col">Редактор</th>
            {showActions && <th scope="col">Действия</th>}
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              <td className="nowrap">{item.number}</td>
              <td>
                {item.title}
                {/* Описание выводится обычной подстановкой: HTML из текста не выполняется. */}
                <span className="row-description">{item.description}</span>
              </td>
              <td>
                {/* Название приходит с сервера вместе с заявкой, а не из файла в коде. */}
                {item.departmentName ?? '—'} {' '}
                <code>{item.departmentId}</code>
              </td>
              <td>
                <span className={`status status--${item.status}`}>{statusLabels[item.status]}</span>
                <code>{item.status}</code>
              </td>
              <td>{item.assigneeFullName ?? <span className="hint">не назначен</span>}</td>
              {showActions && (
                <td>
                  <RowActions
                    item={item}
                    user={user}
                    editors={editors}
                    onAssign={onAssign}
                    onChangeStatus={onChangeStatus}
                    busy={busyId === item.id}
                  />
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function RowActions({ item, user, editors, onAssign, onChangeStatus, busy }) {
  // Координатор назначает редактора, пока заявка не закрыта.
  if (user.role === Role.Coordinator) {
    const closed = item.status === 'Closed' || item.status === 'Cancelled'
    if (closed) return <span className="hint">действий нет</span>

    return (
      <div className="row-actions">
        <label className="visually-hidden" htmlFor={`assignee-${item.id}`}>
          Редактор для заявки {item.number}
        </label>
        <select
          id={`assignee-${item.id}`}
          value={item.assigneeUserId ?? ''}
          disabled={busy}
          onChange={(event) => onAssign(item, Number(event.target.value))}
        >
          <option value="">— выбрать редактора —</option>
          {editors.map((editor) => (
            <option key={editor.id} value={editor.id}>
              {editor.fullName}
            </option>
          ))}
        </select>
      </div>
    )
  }

  // Редактор двигает только свои заявки.
  if (item.assigneeUserId !== user.id) {
    return <span className="hint">назначено не вам</span>
  }

  const next = {
    New: [{ status: 'InProgress', label: 'Взять на проверку' }],
    InProgress: [
      { status: 'Closed', label: 'Опубликовать' },
      { status: 'Cancelled', label: 'Отклонить' },
    ],
  }[item.status]

  if (!next) return <span className="hint">заявка закрыта</span>

  return (
    <div className="row-actions">
      {next.map((action) => (
        <button
          key={action.status}
          type="button"
          disabled={busy}
          onClick={() => onChangeStatus(item, action.status)}
        >
          {action.label}
        </button>
      ))}
    </div>
  )
}

const userShape = PropTypes.shape({
  id: PropTypes.number.isRequired,
  role: PropTypes.string.isRequired,
})

// Обязательный prop: без массива таблицу рисовать нечем.
// defaultProps для функциональных компонентов в React 19 не работает,
// поэтому значение по умолчанию задано прямо в параметрах.
TicketTable.propTypes = {
  items: PropTypes.array.isRequired,
  user: userShape,
  editors: PropTypes.array,
  onAssign: PropTypes.func,
  onChangeStatus: PropTypes.func,
  busyId: PropTypes.number,
}

RowActions.propTypes = {
  item: PropTypes.object.isRequired,
  user: userShape.isRequired,
  editors: PropTypes.array.isRequired,
  onAssign: PropTypes.func.isRequired,
  onChangeStatus: PropTypes.func.isRequired,
  busy: PropTypes.bool,
}
