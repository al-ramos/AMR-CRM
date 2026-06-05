import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Download, TrendingUp, Calendar, Target } from 'lucide-react'
import { relatorioApi } from '../api/relatorioApi'

const fmtBRL = (v: number) =>
  v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 2 })

const STATUS_COLOR: Record<string, string> = {
  Novo:        'bg-secondary',
  Qualificado: 'bg-info text-dark',
  Proposta:    'bg-warning text-dark',
  Ganho:       'bg-success',
  Perdido:     'bg-danger',
}

const ETAPA_COLOR: Record<string, string> = {
  Prospeccao:   'bg-secondary',
  Qualificacao: 'bg-info text-dark',
  Proposta:     'bg-warning text-dark',
  Negociacao:   'bg-primary',
  Fechamento:   'bg-success',
}

const OP_STATUS_COLOR: Record<string, string> = {
  Aberta:      'bg-primary',
  EmAndamento: 'bg-info text-dark',
  Ganha:       'bg-success',
  Perdida:     'bg-danger',
  Cancelada:   'bg-secondary',
}

type Aba = 'funil' | 'forecast' | 'leads'

function downloadCsv(url: string) {
  const a = document.createElement('a')
  a.href = url
  a.click()
}

function PeriodoSelector({
  value, onChange,
}: { value: number; onChange: (v: number) => void }) {
  return (
    <div className="btn-group btn-group-sm" role="group">
      {([30, 60, 90] as const).map(p => (
        <button
          key={p}
          type="button"
          className={`btn ${value === p ? 'btn-primary' : 'btn-outline-primary'}`}
          onClick={() => onChange(p)}
        >
          {p}d
        </button>
      ))}
    </div>
  )
}

// ─── Aba Funil ────────────────────────────────────────────────────────────────

function AbaFunil() {
  const [periodo, setPeriodo] = useState(30)

  const { data, isLoading } = useQuery({
    queryKey: ['relatorio-funil', periodo],
    queryFn:  () => relatorioApi.funil(periodo),
  })

  const maxTotal = Math.max(...(data?.itens.map(i => i.total) ?? [1]), 1)

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div>
          <span className="text-muted small me-2">Período:</span>
          <PeriodoSelector value={periodo} onChange={setPeriodo} />
        </div>
        <button
          className="btn btn-sm btn-outline-success d-flex align-items-center gap-1"
          onClick={() => downloadCsv(relatorioApi.exportFunilUrl(periodo))}
        >
          <Download size={14} /> Export CSV
        </button>
      </div>

      {isLoading ? (
        <p className="text-muted small">Carregando...</p>
      ) : !data || data.itens.length === 0 ? (
        <p className="text-muted small">Sem dados para o período selecionado.</p>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-sm table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>Etapa</th>
                  <th className="text-center">Total Leads</th>
                  <th className="text-end">Valor Estimado</th>
                  <th className="text-center" style={{ minWidth: 160 }}>Taxa Conversão</th>
                  <th style={{ minWidth: 200 }}>Progresso</th>
                </tr>
              </thead>
              <tbody>
                {data.itens.map(item => (
                  <tr key={item.etapa}>
                    <td>
                      <span className={`badge ${STATUS_COLOR[item.etapa] ?? 'bg-secondary'}`}>
                        {item.etapa}
                      </span>
                    </td>
                    <td className="text-center fw-semibold">{item.total}</td>
                    <td className="text-end">{fmtBRL(item.valor)}</td>
                    <td className="text-center">
                      <span className="fw-bold">{item.taxaConversao.toFixed(1)}%</span>
                    </td>
                    <td>
                      <div className="progress" style={{ height: 8 }}>
                        <div
                          className="progress-bar"
                          style={{
                            width: `${(item.total / maxTotal) * 100}%`,
                            background: item.etapa === 'Ganho' ? '#198754' : '#0d6efd',
                          }}
                        />
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot className="table-light">
                <tr>
                  <td colSpan={3} className="fw-semibold">Taxa de Conversão Geral (Novo → Ganho)</td>
                  <td className="text-center fw-bold text-success">
                    {data.taxaConversaoGeral.toFixed(1)}%
                  </td>
                  <td />
                </tr>
              </tfoot>
            </table>
          </div>
        </>
      )}
    </div>
  )
}

// ─── Aba Forecast ─────────────────────────────────────────────────────────────

function AbaForecast() {
  const hoje = new Date()
  const mesAtual = `${hoje.getFullYear()}-${String(hoje.getMonth() + 1).padStart(2, '0')}`
  const [mes, setMes] = useState(mesAtual)

  const { data, isLoading } = useQuery({
    queryKey: ['relatorio-forecast', mes],
    queryFn:  () => relatorioApi.forecast(mes),
  })

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div className="d-flex align-items-center gap-2">
          <span className="text-muted small">Mês:</span>
          <input
            type="month"
            className="form-control form-control-sm"
            style={{ width: 150 }}
            value={mes}
            onChange={e => setMes(e.target.value)}
          />
        </div>
        <button
          className="btn btn-sm btn-outline-success d-flex align-items-center gap-1"
          onClick={() => downloadCsv(relatorioApi.exportForecastUrl(mes))}
        >
          <Download size={14} /> Export CSV
        </button>
      </div>

      {isLoading ? (
        <p className="text-muted small">Carregando...</p>
      ) : !data ? (
        <p className="text-muted small">Sem dados.</p>
      ) : (
        <>
          {/* KPI cards */}
          <div className="row g-3 mb-3">
            <div className="col-sm-4">
              <div className="card border-0 bg-primary bg-opacity-10 p-3">
                <div className="text-muted small">Oportunidades no mês</div>
                <div className="fs-4 fw-bold text-primary">{data.totalOportunidades}</div>
              </div>
            </div>
            <div className="col-sm-4">
              <div className="card border-0 bg-warning bg-opacity-10 p-3">
                <div className="text-muted small">Receita Esperada</div>
                <div className="fs-5 fw-bold text-warning">{fmtBRL(data.receitaEsperada)}</div>
                <div className="text-muted" style={{ fontSize: '0.72rem' }}>Σ Valor × Prob. (em aberto)</div>
              </div>
            </div>
            <div className="col-sm-4">
              <div className="card border-0 bg-success bg-opacity-10 p-3">
                <div className="text-muted small">Receita Confirmada</div>
                <div className="fs-5 fw-bold text-success">{fmtBRL(data.receitaConfirmada)}</div>
                <div className="text-muted" style={{ fontSize: '0.72rem' }}>Oportunidades Ganhas</div>
              </div>
            </div>
          </div>

          {data.oportunidades.length === 0 ? (
            <p className="text-muted small">Nenhuma oportunidade com previsão neste mês.</p>
          ) : (
            <div className="table-responsive">
              <table className="table table-sm table-hover align-middle mb-0">
                <thead className="table-light">
                  <tr>
                    <th>Título</th>
                    <th>Etapa</th>
                    <th>Status</th>
                    <th className="text-end">Valor</th>
                    <th className="text-center">Prob.</th>
                    <th className="text-end">Val. Ponderado</th>
                    <th className="text-center">Previsão</th>
                  </tr>
                </thead>
                <tbody>
                  {data.oportunidades.map(op => (
                    <tr key={op.id}>
                      <td className="fw-medium" style={{ fontSize: '0.875rem' }}>{op.titulo}</td>
                      <td>
                        <span className={`badge ${ETAPA_COLOR[op.etapa] ?? 'bg-secondary'}`}
                          style={{ fontSize: '0.72rem' }}>
                          {op.etapa}
                        </span>
                      </td>
                      <td>
                        <span className={`badge ${OP_STATUS_COLOR[op.status] ?? 'bg-secondary'}`}
                          style={{ fontSize: '0.72rem' }}>
                          {op.status}
                        </span>
                      </td>
                      <td className="text-end">{fmtBRL(op.valor)}</td>
                      <td className="text-center">
                        <div className="d-flex align-items-center gap-1 justify-content-center">
                          <div className="progress flex-grow-1" style={{ height: 6, maxWidth: 60 }}>
                            <div
                              className="progress-bar bg-info"
                              style={{ width: `${op.probabilidade}%` }}
                            />
                          </div>
                          <span style={{ fontSize: '0.78rem' }}>{op.probabilidade}%</span>
                        </div>
                      </td>
                      <td className="text-end fw-semibold text-primary">{fmtBRL(op.valorPonderado)}</td>
                      <td className="text-center text-muted" style={{ fontSize: '0.8rem' }}>
                        {new Date(op.previsaoFechamento).toLocaleDateString('pt-BR')}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </>
      )}
    </div>
  )
}

// ─── Aba Leads ────────────────────────────────────────────────────────────────

function AbaLeads() {
  const [periodo, setPeriodo] = useState(30)

  const { data, isLoading } = useQuery({
    queryKey: ['relatorio-leads', periodo],
    queryFn:  () => relatorioApi.leads(periodo),
  })

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <div>
          <span className="text-muted small me-2">Período:</span>
          <PeriodoSelector value={periodo} onChange={setPeriodo} />
        </div>
        <button
          className="btn btn-sm btn-outline-success d-flex align-items-center gap-1"
          onClick={() => downloadCsv(relatorioApi.exportLeadsUrl(periodo))}
        >
          <Download size={14} /> Export CSV
        </button>
      </div>

      {isLoading ? (
        <p className="text-muted small">Carregando...</p>
      ) : !data || data.totalLeads === 0 ? (
        <p className="text-muted small">Nenhum lead encontrado no período.</p>
      ) : (
        <>
          <div className="row g-3 mb-3">
            <div className="col-sm-6">
              <div className="card border-0 bg-primary bg-opacity-10 p-3">
                <div className="text-muted small">Total de Leads</div>
                <div className="fs-4 fw-bold text-primary">{data.totalLeads}</div>
                <div className="text-muted" style={{ fontSize: '0.72rem' }}>últimos {data.periodoDias} dias</div>
              </div>
            </div>
            <div className="col-sm-6">
              <div className="card border-0 bg-success bg-opacity-10 p-3">
                <div className="text-muted small">Pipeline Total</div>
                <div className="fs-5 fw-bold text-success">{fmtBRL(data.valorTotalPipeline)}</div>
              </div>
            </div>
          </div>

          <div className="row g-3">
            {/* Por Origem */}
            <div className="col-md-6">
              <div className="card card-kpi p-3 h-100">
                <h6 className="fw-semibold mb-3">Leads por Origem</h6>
                {data.porOrigem.length === 0 ? (
                  <p className="text-muted small">Sem dados.</p>
                ) : (
                  <div className="table-responsive">
                    <table className="table table-sm align-middle mb-0">
                      <thead className="table-light">
                        <tr>
                          <th>Origem</th>
                          <th className="text-center">Total</th>
                          <th className="text-end">Valor</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.porOrigem.map(o => (
                          <tr key={o.origem}>
                            <td>
                              <span className="badge bg-secondary" style={{ fontSize: '0.72rem' }}>
                                {o.origem}
                              </span>
                            </td>
                            <td className="text-center fw-semibold">{o.total}</td>
                            <td className="text-end text-muted" style={{ fontSize: '0.82rem' }}>
                              {fmtBRL(o.valorTotal)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>
            </div>

            {/* Por Status */}
            <div className="col-md-6">
              <div className="card card-kpi p-3 h-100">
                <h6 className="fw-semibold mb-3">Leads por Status</h6>
                {data.porStatus.length === 0 ? (
                  <p className="text-muted small">Sem dados.</p>
                ) : (
                  <div className="table-responsive">
                    <table className="table table-sm align-middle mb-0">
                      <thead className="table-light">
                        <tr>
                          <th>Status</th>
                          <th className="text-center">Total</th>
                          <th className="text-end">Valor</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.porStatus.map(s => (
                          <tr key={s.status}>
                            <td>
                              <span
                                className={`badge ${STATUS_COLOR[s.status] ?? 'bg-secondary'}`}
                                style={{ fontSize: '0.72rem' }}
                              >
                                {s.status}
                              </span>
                            </td>
                            <td className="text-center fw-semibold">{s.total}</td>
                            <td className="text-end text-muted" style={{ fontSize: '0.82rem' }}>
                              {fmtBRL(s.valorTotal)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  )
}

// ─── Page ─────────────────────────────────────────────────────────────────────

export default function RelatoriosPage() {
  const [aba, setAba] = useState<Aba>('funil')

  const tabs: { key: Aba; label: string; icon: React.ReactNode }[] = [
    { key: 'funil',    label: 'Funil de Conversão', icon: <TrendingUp size={15} /> },
    { key: 'forecast', label: 'Forecast Mensal',     icon: <Calendar size={15} /> },
    { key: 'leads',    label: 'Análise de Leads',    icon: <Target size={15} /> },
  ]

  return (
    <div>
      <div className="d-flex align-items-center mb-4">
        <h4 className="fw-bold mb-0">Relatórios CRM</h4>
      </div>

      <div className="card card-kpi p-3">
        {/* Tabs */}
        <ul className="nav nav-tabs mb-4">
          {tabs.map(t => (
            <li key={t.key} className="nav-item">
              <button
                className={`nav-link d-flex align-items-center gap-1 ${aba === t.key ? 'active' : ''}`}
                onClick={() => setAba(t.key)}
              >
                {t.icon} {t.label}
              </button>
            </li>
          ))}
        </ul>

        {aba === 'funil'    && <AbaFunil />}
        {aba === 'forecast' && <AbaForecast />}
        {aba === 'leads'    && <AbaLeads />}
      </div>
    </div>
  )
}
