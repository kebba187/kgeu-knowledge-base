// Учебный mock сервера: повторяет форму будущих ответов ASP.NET Core,
// но живёт в JavaScript. Страницы его не импортируют — только api/tickets.js.

const DELAY_MS = 500

// Учебные переключатели для демонстрации состояний UI.
const flags = { force500: false, forceEmpty: false, skipClientValidation: false }

export function setFlag(name, value) {
  flags[name] = value
}

export function getFlags() {
  return { ...flags }
}

let mockItems = [
  {
    id: 1,
    number: 'БЗ-2026-0001',
    title: 'Порядок оформления академического отпуска',
    departmentId: 1,
    description: 'Какие документы нужны, куда подавать и в какой срок рассматривают.',
    status: 'New',
  },
  {
    id: 2,
    number: 'БЗ-2026-0002',
    title: 'Как получить доступ к ЭИОС',
    departmentId: 4,
    description: 'Где взять логин, что делать при утере пароля.',
    status: 'InProgress',
  },
  {
    id: 3,
    number: 'БЗ-2026-0003',
    title: 'Заселение в общежитие',
    departmentId: 3,
    description: 'Список документов и порядок заселения первокурсников.',
    status: 'Closed',
  },
]

let nextId = 4

function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms))
}

export async function mockGetTickets() {
  await delay(DELAY_MS)
  if (flags.force500) {
    return { ok: false, status: 500, data: null }
  }
  return { ok: true, status: 200, data: flags.forceEmpty ? [] : mockItems }
}

export async function mockCreateTicket(body) {
  await delay(DELAY_MS)
  if (flags.force500) {
    return { ok: false, status: 500, data: null }
  }

  // Те же правила, что потом проверит сервер в ЛР8.
  const title = (body.title ?? '').trim()
  const description = (body.description ?? '').trim()
  const fields = {}
  if (title.length < 5 || title.length > 80) fields.title = 'От 5 до 80 символов'
  if (!Number.isInteger(body.departmentId) || body.departmentId <= 0)
    fields.departmentId = 'Целое число больше нуля'
  if (description.length < 10 || description.length > 500)
    fields.description = 'От 10 до 500 символов'

  if (Object.keys(fields).length > 0) {
    return { ok: false, status: 400, data: { error: 'validation', fields } }
  }

  // Сервер сам задаёт id, номер и статус.
  const created = {
    id: nextId,
    number: `БЗ-2026-${String(nextId).padStart(4, '0')}`,
    title,
    departmentId: body.departmentId,
    description,
    status: 'New',
  }
  nextId += 1
  mockItems = [...mockItems, created]

  return { ok: true, status: 201, data: created }
}
