import { useQuery } from '@tanstack/react-query'
import { Users, TrendingUp, CheckCircle, DollarSign } from 'lucide-react'
import { contatoApi } from '../api/contatoApi'
import { oportunidadeApi, StatusOportunidade } from '../api/oportunidadeApi'

export default function DashboardPage() {
  const { data: contatos = [] } = useQuery({
    queryKey: ['contatos'],
    queryFn: contatoApi.listar,
  })
  const { data: oportunidades = [] } = useQuery({
    queryKey: ['oportunidades'],
    queryFn: oportunidadeApi.listar,
  })

  const totalValor = oportunidades.reduce((s, o) => s + o.valor, 0)
  const ganhas = oportunidades.filter(o => o.status === StatusOportunidade.Ganha).length
  const abertas = oportunidades.filter(o => o.status === StatusOportunidade.Aberta).length

  const kpis = [
    { label: 'Contatos',        value: contatos.length,          icon: <Users size={28} />,       color: '#0d6efd' },
    { label: 'Oportunidades',   value: oportunidades.length,     icon: <TrendingUp size={28} />,  color: '#fd7e14' },
    { label: 'Ganhas',          value: ganhas,                   icon: <CheckCircle size={28} />, color: '#198754' },
    { label: 'Pipeline (R$)',   value: `R$ ${totalValor.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}`,
                                                                  icon: <DollarSign size={28} />,  color: '#6f42c1' },
  ]

  return (
    <div>
      <h4 className="fw-bold mb-4">Dashboard</h4>

      <div className="row g-3 mb-4">
        {kpis.map(kpi => (
          <div key={kpi.label} className="col-sm-6 col-xl-3">
            <div className="card card-kpi p-3">
              <div className="d-flex align-items-center gap-3">
                <div className="rounded-3 p-2" style={{ background: kpi.color + '20', color: kpi.color }}>
                  {kpi.icon}
                </div>
                <div>
                  <div className="text-muted small">{kpi.label}</div>
                  <div className="fs-4 fw-bold">{kpi.value}</div>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="row g-3">
        <div className="col-lg-6">
          <div className="card card-kpi p-3">
            <h6 className="fw-semibold mb-3">Oportunidades Abertas ({abertas})</h6>
            {oportunidades
              .filter(o => o.status === StatusOportunidade.Aberta)
              .slice(0, 5)
              .map(o => (
                <div key={o.id} className="d-flex justify-content-between align-items-center py-2 border-bottom">
                  <div>
                    <div className="fw-medium" style={{ fontSize: '0.9rem' }}>{o.titulo}</div>
                    <small className="text-muted">{o.contatoNome}</small>
                  </div>
                  <span className="badge bg-primary">
                    R$ {o.valor.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}
                  </span>
                </div>
              ))}
            {abertas === 0 && <p className="text-muted small mb-0">Nenhuma oportunidade aberta.</p>}
          </div>
        </div>

        <div className="col-lg-6">
          <div className="card card-kpi p-3">
            <h6 className="fw-semibold mb-3">Últimos Contatos</h6>
            {contatos.slice(0, 5).map(c => (
              <div key={c.id} className="d-flex align-items-center gap-3 py-2 border-bottom">
                <div className="rounded-circle d-flex align-items-center justify-content-center text-white fw-bold"
                  style={{ width: 36, height: 36, background: '#0d6efd', fontSize: '0.8rem', flexShrink: 0 }}>
                  {c.nome.charAt(0).toUpperCase()}
                </div>
                <div>
                  <div className="fw-medium" style={{ fontSize: '0.9rem' }}>{c.nome}</div>
                  <small className="text-muted">{c.empresa ?? c.email}</small>
                </div>
                <span className={`badge ms-auto ${c.status === 1 ? 'bg-success' : 'bg-secondary'}`}>
                  {c.statusNome}
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}
