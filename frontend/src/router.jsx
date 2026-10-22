import { Route, Routes } from 'react-router-dom'
import Login from './pages/Login.jsx'
import TicketList from './pages/TicketList.jsx'
import TicketNew from './pages/TicketNew.jsx'

// Аналог router.js из методички: таблица «адрес → страница».
// <Routes> играет роль <router-view>: сюда подставляется страница текущего адреса.
export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<TicketList />} />
      <Route path="/new" element={<TicketNew />} />
      <Route path="/login" element={<Login />} />
    </Routes>
  )
}
