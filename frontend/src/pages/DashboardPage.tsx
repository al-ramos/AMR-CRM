import { useQuery } from '@tanstack/react-query'
import { Users, TrendingUp, CheckCircle, Target } from 'lucide-react'
import { contatoApi } from '../api/contatoApi'
import { oportunidadeApi, StatusOportunidade } from '../api/oportunidadeApi'
import { leadApi, StatusLead } from '../api/leadApi'

const fmtBRL = (v: number) =>
  v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 })

const STATUS_LABEL: Record<number, string> = {
  1: 'Novo', 2: 'Qualificado', 3: 'Proposta', 4: 'Ganho', 5: 'Perdido',
}
const STATUS_DOT: Record<number, string> = {
  1: '#6c757d', 2: '#0dcaf0', 3: '#ffc107', 4: '#198754', 5: '#dc3545',
}

export default function DashboardPage() {
  const { data: contatos = [] } = useQuery({ queryKey: ['contatos'], queryFn: contatoApi.listar })
  const { data: leads    = [] } = useQuery({ queryKey: ['leads'],    queryFn: () => leadApi.listar() })
  const { data: ops      = [] } = useQuery({ queryKey: ['oportunidades'], queryFn: oportunidadeApi.listar })

  const leadsAtivos    = leads.filter(l => l.status !== StatusLead.Ganho && l.status !== StatusLead.Perdido)
  const pipeline       = leadsAtivos.reduce((s, l) => s + l.valorEstimado, 0)
  const ganhas         = ops.filter(o => o.status === StatusOportunidade.Ganha).length
  const pipelineOp     = ops
    .filter(o => o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento)
    .reduce((s, o) => s + o.valor * (o.probabilidade / 100), 0)

  const kpis = [
    { label: 'Leads ativos',  value: leadsAtivos.length, icon: <Target size={26} />,     color: '#fd7e14' },
    { label: 'Contatos',       value: contatos.length,    icon: <Users size={26} />,       color: '#0d6efd' },
    { label: 'Op. ganhas',     value: ganhas,             icon: <CheckCircle size={26} />, color: '#198754' },
    { label: 'Pipeline leads', value: fmtBRL(pipeline),  icon: <TrendingUp size={26} />,  color: '#6f42c1' },
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
        {/* Leads por status */}
        <div className="col-lg-5">
          <div className="card card-kpi p-3 h-100">
            <h6 className="fw-semibold mb-3">Leads por status</h6>
            {[1,2,3,4,5].map(s => {
              const count = leads.filter(l => l.status === s).length
              const pct   = leads.length ? Math.round(count / leads.length * 100) : 0
              return (
                <div key={s} className="mb-2">
                  <div className="d-flex justify-content-between mb-1">
                    <small className="fw-medium" style={{ color: STATUS_DOT[s] }}>
                      ● {STATUS_LABEL[s]}
                    </small>
                    <small className="text-muted">{count}</small>
                  </div>
                  <div className="progress" style={{ height: 6 }}>
                    <div className="progress-bar"
                      style={{ width: `${pct}%`, background: STATUS_DOT[s] }} />
                  </div>
                </div>
              )
            })}
          </div>
        </div>

        {/* Pipeline Oportunidades */}
        <div className="col-lg-7">
          <div className="card card-kpi p-3 h-100">
            <div className="d-flex justify-content-between align-items-center mb-3">
              <h6 className="fw-semibold mb-0">Oportunidades em andamento</h6>
              <span className="badge bg-primary">{fmtBRL(pipelineOp)} ponderado</span>
            </div>
            {ops
              .filter(o => o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento)
              .slice(0, 6)
              .map(o => (
                <div key={o.id} className="d-flex justify-content-between align-items-center py-2 border-bottom">
                  <div>
                    <div className="fw-medium" style={{ fontSize: '0.88rem' }}>{o.titulo}</div>
                    <small className="text-muted">{o.leadNome ?? o.contatoNome ?? '—'}</small>
                  </div>
                  <div className="text-end">
                    <div className="small fw-medium">{fmtBRL(o.valor)}</div>
                    <small className="text-muted">{o.probabilidade}%</small>
                  </div>
                </div>
              ))}
            {ops.filter(o => o.status <= 2).length === 0 && (
              <p className="text-muted small mb-0">Nenhuma oportunidade em aberto.</p>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
