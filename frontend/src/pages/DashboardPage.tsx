import { useQuery } from '@tanstack/react-query'
import { Users, TrendingUp, DollarSign, Percent } from 'lucide-react'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from 'recharts'
import { dashboardApi, type KanbanColunaDto } from '../api/dashboardApi'

const fmt = (v: number) =>
  v.toLocaleString('pt-BR', { minimumFractionDigits: 0, maximumFractionDigits: 2 })

const COLUNA_CORES: Record<number, { bg: string; badge: string }> = {
  1: { bg: '#0d6efd', badge: 'bg-primary' },
  2: { bg: '#fd7e14', badge: 'bg-warning text-dark' },
  3: { bg: '#198754', badge: 'bg-success' },
  4: { bg: '#dc3545', badge: 'bg-danger' },
  5: { bg: '#6c757d', badge: 'bg-secondary' },
}

function KanbanColuna({ col }: { col: KanbanColunaDto }) {
  const cor = COLUNA_CORES[col.status] ?? { bg: '#0d6efd', badge: 'bg-primary' }
  return (
    <div style={{ minWidth: 200, flex: '0 0 200px' }}>
      <div
        className="rounded-top text-white px-3 py-2 d-flex justify-content-between align-items-center"
        style={{ background: cor.bg }}
      >
        <span className="fw-semibold small">{col.statusNome}</span>
        <span className="badge bg-white text-dark">{col.total}</span>
      </div>
      <div className="border border-top-0 rounded-bottom p-2 d-flex flex-column gap-2"
        style={{ minHeight: 120, background: '#f8f9fa' }}>
        {col.cards.length === 0 && (
          <p className="text-muted small text-center my-auto pt-2">Vazio</p>
        )}
        {col.cards.map(card => (
          <div key={card.id} className="card border-0 shadow-sm p-2">
            <div className="fw-medium" style={{ fontSize: '0.8rem' }}>{card.titulo}</div>
            <div className="text-muted" style={{ fontSize: '0.75rem' }}>{card.contatoNome}</div>
            {card.valor > 0 && (
              <span className={`badge mt-1 align-self-start ${cor.badge}`} style={{ fontSize: '0.7rem' }}>
                R$ {fmt(card.valor)}
              </span>
            )}
          </div>
        ))}
        {col.total > col.cards.length && (
          <p className="text-muted small text-center mb-0">
            +{col.total - col.cards.length} mais
          </p>
        )}
      </div>
    </div>
  )
}

export default function DashboardPage() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['dashboard'],
    queryFn: dashboardApi.obter,
    refetchInterval: 30_000,
  })

  if (isLoading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ height: 300 }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Carregando...</span>
        </div>
      </div>
    )
  }

  if (isError || !data) {
    return (
      <div className="alert alert-danger">
        Não foi possível carregar o dashboard. Tente novamente.
      </div>
    )
  }

  const kpis = [
    {
      label: 'Total Leads',
      value: data.totalContatos,
      icon: <Users size={26} />,
      color: '#0d6efd',
    },
    {
      label: 'Valor do Pipeline',
      value: `R$ ${fmt(data.valorPipeline)}`,
      icon: <DollarSign size={26} />,
      color: '#198754',
    },
    {
      label: 'Taxa de Conversão',
      value: `${data.taxaConversao.toLocaleString('pt-BR')}%`,
      icon: <Percent size={26} />,
      color: '#fd7e14',
    },
    {
      label: 'Ticket Médio',
      value: `R$ ${fmt(data.ticketMedio)}`,
      icon: <TrendingUp size={26} />,
      color: '#6f42c1',
    },
  ]

  return (
    <div>
      <h4 className="fw-bold mb-4">Dashboard</h4>

      {/* KPI Cards */}
      <div className="row g-3 mb-4">
        {kpis.map(kpi => (
          <div key={kpi.label} className="col-sm-6 col-xl-3">
            <div className="card card-kpi p-3">
              <div className="d-flex align-items-center gap-3">
                <div
                  className="rounded-3 p-2"
                  style={{ background: kpi.color + '20', color: kpi.color }}
                >
                  {kpi.icon}
                </div>
                <div>
                  <div className="text-muted small">{kpi.label}</div>
                  <div className="fs-5 fw-bold">{kpi.value}</div>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Kanban */}
      <div className="card card-kpi p-3 mb-4">
        <h6 className="fw-semibold mb-3">
          Pipeline Kanban
          <span className="text-muted fw-normal ms-2" style={{ fontSize: '0.85rem' }}>
            ({data.totalOportunidades} oportunidades)
          </span>
        </h6>
        {data.totalOportunidades === 0 ? (
          <p className="text-muted small mb-0">Nenhuma oportunidade cadastrada.</p>
        ) : (
          <div className="d-flex gap-3 overflow-auto pb-2">
            {data.kanbanColunas.map(col => (
              <KanbanColuna key={col.status} col={col} />
            ))}
          </div>
        )}
      </div>

      {/* Gráfico de barras */}
      <div className="card card-kpi p-3">
        <h6 className="fw-semibold mb-3">Oportunidades Criadas por Mês</h6>
        {data.oportunidadesPorMes.every(m => m.total === 0) ? (
          <p className="text-muted small mb-0">Nenhuma oportunidade nos últimos 6 meses.</p>
        ) : (
          <ResponsiveContainer width="100%" height={220}>
            <BarChart data={data.oportunidadesPorMes} margin={{ top: 4, right: 16, left: 0, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} />
              <XAxis dataKey="mes" tick={{ fontSize: 12 }} />
              <YAxis allowDecimals={false} tick={{ fontSize: 12 }} />
              <Tooltip
                formatter={(value: number) => [value, 'Oportunidades']}
                labelStyle={{ fontWeight: 600 }}
              />
              <Bar dataKey="total" fill="#0d6efd" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  )
}
