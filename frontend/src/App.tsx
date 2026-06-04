import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import Sidebar from './components/Sidebar'
import DashboardPage from './pages/DashboardPage'
import LeadsPage from './pages/LeadsPage'
import ContatosPage from './pages/ContatosPage'
import OportunidadesPage from './pages/OportunidadesPage'

export default function App() {
  return (
    <BrowserRouter>
      <Sidebar />
      <main className="main-content">
        <Routes>
          <Route path="/"              element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard"     element={<DashboardPage />} />
          <Route path="/leads"         element={<LeadsPage />} />
          <Route path="/contatos"      element={<ContatosPage />} />
          <Route path="/oportunidades" element={<OportunidadesPage />} />
        </Routes>
      </main>
    </BrowserRouter>
  )
}
