import TicketTable from '../components/TicketTable.jsx'
import { useTickets } from '../ticketsStore.js'

// Страница владеет данными и передаёт их таблице через props.
export default function TicketList() {
  const items = useTickets()

  return (
    <main>
      <section aria-labelledby="list-heading">
        <h1 id="list-heading">Заявки на публикацию</h1>
        <TicketTable items={items} />
      </section>
    </main>
  )
}
