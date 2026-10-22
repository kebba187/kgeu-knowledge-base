import PropTypes from 'prop-types'
import { useState } from 'react'
import { apiInfo, getDemoFlags, setDemoFlag } from '../api/tickets.js'

// Учебный блок: позволяет показать состояния «пусто» и «ошибка 500» без сервера.
// Со включением настоящего API (USE_MOCK = false) блок не отображается.
export default function DemoSwitches({ onChange }) {
  const [flags, setFlags] = useState(getDemoFlags())

  if (!apiInfo.useMock) return null

  function toggle(name) {
    const next = !flags[name]
    setDemoFlag(name, next)
    setFlags(getDemoFlags())
    onChange?.()
  }

  return (
    <p className="demo-toggle">
      Учебные отказы mock:
      <label htmlFor="forceEmpty">
        <input
          id="forceEmpty"
          type="checkbox"
          checked={flags.forceEmpty}
          onChange={() => toggle('forceEmpty')}
        />
        пустой список
      </label>
      <label htmlFor="skipClientValidation">
        <input
          id="skipClientValidation"
          type="checkbox"
          checked={flags.skipClientValidation}
          onChange={() => toggle('skipClientValidation')}
        />
        без клиентской проверки (демо 400)
      </label>
      <label htmlFor="force500">
        <input
          id="force500"
          type="checkbox"
          checked={flags.force500}
          onChange={() => toggle('force500')}
        />
        ответ 500
      </label>
    </p>
  )
}

DemoSwitches.propTypes = {
  // Необязательный обработчик: страница списка перезагружает данные после смены флага.
  onChange: PropTypes.func,
}
