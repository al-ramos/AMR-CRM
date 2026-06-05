import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip,
  Legend, ResponsiveContainer, Cell,
} from 'recharts'
import { TrendingUp, Target, Briefcase, Award } from 'lucide-react'
import { dashboardApi } from '../api/dashboardApi'

const fmtBRL = (v: number) =>
  v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 })

const ETAPA_COLOR: Record<string, string> = {
  Prospeccao:   '#0d6efd',
  Qualificacao: '#6f42c1',
  Proposta:     '#fd7e14',
  Negociacao:   '#ffc107',
  Fechamento:   '#198754',
}

const STATUS_BADGE: Record<string, string> = {
  Aberta:     'bg-primary',
  EmAndamento:'bg-info text-dark',
  Ganha:      'bg-success',
  Perdida:    'bg-danger',
  Cancelada:  'bg-secondary',
}

export default function DashboardPage() {
  const [periodo, setPeriodo] = useState(30)

  const { data: kpis } = useQuery({
    queryKey: ['dashboard-kpis'],
    queryFn:  dashboardApi.kpis,
  })
  const { data: funil } = useQuery({
    queryKey: ['dashboard-funil'],
    queryFn:  dashboardApi.funil,
  })
  const { data: topOps = [] } = useQuery({
    queryKey: ['dashboard-top'],
    queryFn:  dashboardApi.topOportunidades,
  })
  const { data: evolucao = [] } = useQuery({
    queryKey: ['dashboard-evolucao', periodo],
    queryFn:  () => dashboardApi.evolucao(periodo),
  })

  const kpiCards = [
    {
      label: 'Total Leads',
      value: kpis?.totalLeads ?? 0,
      sub: `${kpis?.leadsAtivos ?? 0} ativos`,
      icon: <Target size={26} />,
      color: '#fd7e14',
    },
    {
      label: 'Op. Abertas',
      value: kpis?.oportunidadesAbertas ?? 0,
      sub: 'em andamento',
      icon: <Briefcase size={26} />,
      color: '#0d6efd',
    },
    {
      label: 'Pipeline Ponderado',
      value: fmtBRL(kpis?.pipelinePonderado ?? 0),
      sub: 'Σ Valor × Prob.',
      icon: <TrendingUp size={26} />,
      color: '#6f42c1',
    },
    {
      label: 'Taxa de Conversão',
      value: `${kpis?.taxaConversao ?? 0}%`,
      sub: 'leads → ganhos',
      icon: <Award size={26} />,
      color: '#198754',
    },
  ]

  const funilData = funil?.oportunidades.map(e => ({
    name:  e.etapa,
    count: e.count,
    valor: e.valor,
  })) ?? []

  return (
    <div>
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h4 className="fw-bold mb-0">Dashboard CRM</h4>
        <div className="btn-group btn-group-sm" role="group" aria-label="Período">
          {([30, 60, 90] as const).map(p => (
            <button
              key={p}
              type="button"
              className={`btn ${periodo === p ? 'btn-primary' : 'btn-outline-primary'}`}
              onClick={() => setPeriodo(p)}
            >
              {p}d
            </button>
          ))}
        </div>
      </div>

      {/* KPI Cards */}
      <div className="row g-3 mb-4">
        {kpiCards.map(kpi => (
          <div key={kpi.label} className="col-sm-6 col-xl-3">
            <div className="card card-kpi p-3 h-100">
              <div className="d-flex align-items-center gap-3">
                <div
                  className="rounded-3 p-2 flex-shrink-0"
                  style={{ background: kpi.color + '20', color: kpi.color }}
                >
                  {kpi.icon}
                </div>
                <div>
                  <div className="text-muted small">{kpi.label}</div>
                  <div className="fs-4 fw-bold">{kpi.value}</div>
                  <div className="text-muted" style={{ fontSize: '0.75rem' }}>{kpi.sub}</div>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Charts Row */}
      <div className="row g-3 mb-4">
        {/* Funil por Etapa */}
        <div className="col-lg-6">
          <div className="card card-kpi p-3 h-100">
            <h6 className="fw-semibold mb-3">Funil por Etapa (Oportunidades)</h6>
            {funilData.length === 0 ? (
              <p className="text-muted small mb-0">Sem dados.</p>
            ) : (
              <ResponsiveContainer width="100%" height={220}>
                <BarChart data={funilData} layout="vertical"
                  margin={{ top: 0, right: 16, left: 16, bottom: 0 }}>
                  <CartesianGrid strokeDasharray="3 3" horizontal={false} stroke="#f0f0f0" />
                  <XAxis type="number" tick={{ fontSize: 11 }} allowDecimals={false} />
                  <YAxis type="category" dataKey="name" tick={{ fontSize: 11 }} width={90} />
                  <Tooltip
                    formatter={(v) => [v, 'Qtd.']}
                    labelFormatter={(l) => `Etapa: ${l}`}
                  />
                  <Bar dataKey="count" name="Oportunidades" radius={[0, 4, 4, 0]} maxBarSize={28}>
                    {funilData.map((entry, i) => (
                      <Cell key={i} fill={ETAPA_COLOR[entry.name] ?? '#6c757d'} />
                    ))}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            )}
          </div>
        </div>

        {/* Evolução por Mês */}
        <div className="col-lg-6">
          <div className="card card-kpi p-3 h-100">
            <h6 className="fw-semibold mb-3">Evolução por Mês ({periodo}d)</h6>
            {evolucao.length === 0 ? (
              <p className="text-muted small mb-0">Sem dados para o período.</p>
            ) : (
              <ResponsiveContainer width="100%" height={220}>
                <BarChart data={evolucao} margin={{ top: 0, right: 16, left: -10, bottom: 0 }}>
                  <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f0f0f0" />
                  <XAxis dataKey="mes" tick={{ fontSize: 11 }} />
                  <YAxis tick={{ fontSize: 11 }} allowDecimals={false} />
                  <Tooltip />
                  <Legend wrapperStyle={{ fontSize: 12 }} />
                  <Bar dataKey="abertas" name="Abertas"  fill="#0d6efd" radius={[4, 4, 0, 0]} maxBarSize={32} />
                  <Bar dataKey="fechadas" name="Fechadas" fill="#198754" radius={[4, 4, 0, 0]} maxBarSize={32} />
                </BarChart>
              </ResponsiveContainer>
            )}
          </div>
        </div>
      </div>

      {/* Top 5 Oportunidades */}
      <div className="card card-kpi p-3">
        <h6 className="fw-semibold mb-3">Top 5 Oportunidades por Valor</h6>
        {topOps.length === 0 ? (
          <p className="text-muted small mb-0">Nenhuma oportunidade encontrada.</p>
        ) : (
          <div className="table-responsive">
            <table className="table table-sm table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>Título</th>
                  <th>Lead / Contato</th>
                  <th>Etapa</th>
                  <th>Status</th>
                  <th className="text-end">Valor</th>
                  <th className="text-end">Ponderado</th>
                  <th className="text-end">Prob.</th>
                </tr>
              </thead>
              <tbody>
                {topOps.map(op => (
                  <tr key={op.id}>
                    <td className="fw-medium" style={{ fontSize: '0.875rem' }}>{op.titulo}</td>
                    <td className="text-muted" style={{ fontSize: '0.8rem' }}>
                      {op.lead ?? op.contato ?? '—'}
                    </td>
                    <td>
                      <span
                        className="badge"
                        style={{ background: ETAPA_COLOR[op.etapa] ?? '#6c757d', fontSize: '0.72rem' }}
                      >
                        {op.etapa}
                      </span>
                    </td>
                    <td>
                      <span className={`badge ${STATUS_BADGE[op.status] ?? 'bg-secondary'}`}
                        style={{ fontSize: '0.72rem' }}>
                        {op.status}
                      </span>
                    </td>
                    <td className="text-end" style={{ fontSize: '0.875rem' }}>{fmtBRL(op.valor)}</td>
                    <td className="text-end text-muted" style={{ fontSize: '0.8rem' }}>
                      {fmtBRL(op.valorPonderado)}
                    </td>
                    <td className="text-end text-muted" style={{ fontSize: '0.8rem' }}>
                      {op.probabilidade}%
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
