import PropTypes from 'prop-types'
import { departmentName, statusLabels } from '../dictionaries.js'

// Таблица только рисует то, что ей передали через props.
// Своего массива у неё нет и копию входного массива она не делает.
export default function TicketTable({ items }) {
  return (
    <div className="table-wrap">
      <table>
        <caption>Заявки на публикацию материалов базы знаний</caption>
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
                <span className={`status status--${item.status}`}>{statusLabels[item.status]}</span>
                <code>{item.status}</code>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

// Обязательный prop: без массива таблицу рисовать нечем.
TicketTable.propTypes = {
  items: PropTypes.array.isRequired,
}
