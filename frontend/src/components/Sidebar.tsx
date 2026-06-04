import { NavLink } from 'react-router-dom'
import { Users, TrendingUp, LayoutDashboard, UserSearch } from 'lucide-react'

export default function Sidebar() {
  return (
    <nav className="sidebar py-3">
      <div className="px-3 mb-4">
        <h5 className="text-white fw-bold mb-0">
          <span className="text-primary">AMR</span> CRM
        </h5>
        <small style={{ fontSize: '0.7rem', opacity: 0.5 }}>Sprint 7</small>
      </div>

      <ul className="nav flex-column gap-1">
        <li className="nav-item">
          <NavLink to="/dashboard" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <LayoutDashboard size={16} />
            Dashboard
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/leads" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <UserSearch size={16} />
            Leads
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/contatos" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <Users size={16} />
            Contatos
          </NavLink>
        </li>
        <li className="nav-item">
          <NavLink to="/oportunidades" className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}>
            <TrendingUp size={16} />
            Oportunidades
          </NavLink>
        </li>
      </ul>

      <div className="mt-auto px-3 py-3" style={{ opacity: 0.35, fontSize: '0.7rem' }}>
        AMR SYSTEM v1.0
      </div>
    </nav>
  )
}
