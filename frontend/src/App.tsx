import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import Sidebar from './components/Sidebar'
import DashboardPage from './pages/DashboardPage'
import ContatosPage from './pages/ContatosPage'
import OportunidadesPage from './pages/OportunidadesPage'
import OportunidadeDetalhePage from './pages/OportunidadeDetalhePage'
import LeadsPage from './pages/LeadsPage'

export default function App() {
  return (
    <BrowserRouter>
      <Sidebar />
      <main className="main-content">
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/leads" element={<LeadsPage />} />
          <Route path="/contatos" element={<ContatosPage />} />
          <Route path="/oportunidades" element={<OportunidadesPage />} />
          <Route path="/oportunidades/:id" element={<OportunidadeDetalhePage />} />
        </Routes>
      </main>
    </BrowserRouter>
  )
}
