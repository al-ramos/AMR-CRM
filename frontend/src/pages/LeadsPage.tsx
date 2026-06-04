import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { PlusCircle, Pencil, Trash2 } from 'lucide-react'
import {
  leadApi, LeadDto, CriarLeadRequest,
  StatusLead, ORIGENS,
} from '../api/leadApi'

// ── badge ────────────────────────────────────────────────────────────────────
const STATUS_LABEL: Record<number, string> = {
  1: 'Novo', 2: 'Qualificado', 3: 'Proposta', 4: 'Ganho', 5: 'Perdido',
}
const STATUS_CLASS: Record<number, string> = {
  1: 'bg-secondary',
  2: 'bg-info text-dark',
  3: 'bg-warning text-dark',
  4: 'bg-success',
  5: 'bg-danger',
}

// ── helpers ──────────────────────────────────────────────────────────────────
const fmtBRL = (v: number) =>
  v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 })

const PROXIMOS: Record<number, number[]> = {
  1: [2, 4, 5],
  2: [3, 4, 5],
  3: [4, 5],
  4: [],
  5: [],
}

const EMPTY: CriarLeadRequest = { nome: '', email: '', origem: 1, valorEstimado: 0 }

export default function LeadsPage() {
  const qc = useQueryClient()
  const [filtroStatus, setFiltroStatus] = useState<number | undefined>(undefined)
  const [showModal, setShowModal]       = useState(false)
  const [editing, setEditing]           = useState<LeadDto | null>(null)
  const [form, setForm]                 = useState<CriarLeadRequest>(EMPTY)

  const { data: leads = [], isLoading } = useQuery({
    queryKey: ['leads', filtroStatus],
    queryFn:  () => leadApi.listar(filtroStatus),
  })

  const criarMut = useMutation({
    mutationFn: leadApi.criar,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['leads'] }); closeModal() },
  })
  const atualizarMut = useMutation({
    mutationFn: ({ id, req }: { id: string; req: CriarLeadRequest }) => leadApi.atualizar(id, req),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['leads'] }); closeModal() },
  })
  const statusMut = useMutation({
    mutationFn: ({ id, s }: { id: string; s: number }) => leadApi.avancarStatus(id, s),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['leads'] }),
  })
  const excluirMut = useMutation({
    mutationFn: leadApi.excluir,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['leads'] }),
  })

  const openCreate = () => { setEditing(null); setForm(EMPTY); setShowModal(true) }
  const openEdit   = (l: LeadDto) => {
    setEditing(l)
    setForm({
      nome: l.nome, email: l.email, origem: l.origem,
      telefone: l.telefone, empresa: l.empresa,
      valorEstimado: l.valorEstimado, notas: l.notas,
    })
    setShowModal(true)
  }
  const closeModal = () => { setShowModal(false); setEditing(null); setForm(EMPTY) }

  const handleSave = () => {
    if (editing) atualizarMut.mutate({ id: editing.id, req: form })
    else criarMut.mutate(form)
  }

  const isWorking = criarMut.isPending || atualizarMut.isPending

  // pipeline total (excluindo Ganho e Perdido)
  const pipeline = leads
    .filter(l => l.status !== StatusLead.Ganho && l.status !== StatusLead.Perdido)
    .reduce((s, l) => s + l.valorEstimado, 0)

  return (
    <div>
      {/* ── cabeçalho ── */}
      <div className="d-flex align-items-center justify-content-between mb-3">
        <h4 className="fw-bold mb-0">Leads</h4>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2"
          onClick={openCreate}>
          <PlusCircle size={15} /> Novo Lead
        </button>
      </div>

      {/* ── filtro de status ── */}
      <div className="d-flex gap-2 flex-wrap mb-3">
        {[undefined, 1, 2, 3, 4, 5].map(s => (
          <button key={s ?? 'all'}
            className={`btn btn-sm ${filtroStatus === s
              ? (s ? STATUS_CLASS[s].replace('text-dark', '').trim() : 'btn-dark')
              : 'btn-outline-secondary'}`}
            onClick={() => setFiltroStatus(s)}>
            {s ? STATUS_LABEL[s] : 'Todos'}
          </button>
        ))}
      </div>

      {/* ── tabela ── */}
      <div className="card card-kpi">
        {isLoading ? (
          <div className="p-4 text-center text-muted">Carregando...</div>
        ) : (
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>Nome</th>
                  <th>Empresa</th>
                  <th>Origem</th>
                  <th>Valor Est.</th>
                  <th>Status</th>
                  <th>Avançar</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {leads.map(l => (
                  <tr key={l.id} style={{ opacity: l.status >= 4 ? 0.65 : 1 }}>
                    <td>
                      <div className="fw-medium">{l.nome}</div>
                      <small className="text-muted">{l.email}</small>
                    </td>
                    <td className="small">{l.empresa ?? '—'}</td>
                    <td className="small">{l.origemNome}</td>
                    <td className="small fw-medium">{fmtBRL(l.valorEstimado)}</td>
                    <td>
                      <span className={`badge ${STATUS_CLASS[l.status]}`}>
                        {STATUS_LABEL[l.status]}
                      </span>
                    </td>
                    <td>
                      <div className="d-flex gap-1 flex-wrap">
                        {PROXIMOS[l.status]?.map(ns => (
                          <button key={ns}
                            className={`btn btn-sm ${STATUS_CLASS[ns].includes('danger')
                              ? 'btn-outline-danger'
                              : STATUS_CLASS[ns].includes('success')
                              ? 'btn-outline-success'
                              : 'btn-outline-secondary'}`}
                            style={{ fontSize: '0.75rem' }}
                            onClick={() => statusMut.mutate({ id: l.id, s: ns })}>
                            → {STATUS_LABEL[ns]}
                          </button>
                        ))}
                      </div>
                    </td>
                    <td>
                      <div className="d-flex gap-1">
                        <button className="btn btn-outline-primary btn-sm"
                          onClick={() => openEdit(l)}>
                          <Pencil size={13} />
                        </button>
                        {l.status !== StatusLead.Ganho && (
                          <button className="btn btn-outline-danger btn-sm"
                            onClick={() => excluirMut.mutate(l.id)}>
                            <Trash2 size={13} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
                {leads.length === 0 && (
                  <tr>
                    <td colSpan={7} className="text-center text-muted py-4">
                      Nenhum lead encontrado.
                    </td>
                  </tr>
                )}
              </tbody>
              {leads.length > 0 && (
                <tfoot className="table-light">
                  <tr>
                    <td colSpan={3} className="fw-semibold small">Pipeline ativo</td>
                    <td className="fw-bold text-primary small">{fmtBRL(pipeline)}</td>
                    <td colSpan={3}></td>
                  </tr>
                </tfoot>
              )}
            </table>
          </div>
        )}
      </div>

      {/* ── modal criar / editar ── */}
      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{editing ? 'Editar Lead' : 'Novo Lead'}</h5>
                <button className="btn-close" onClick={closeModal} />
              </div>
              <div className="modal-body">
                <div className="row g-3">
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Nome *</label>
                    <input className="form-control" value={form.nome}
                      onChange={e => setForm(f => ({ ...f, nome: e.target.value }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">E-mail *</label>
                    <input className="form-control" type="email" value={form.email}
                      onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Empresa</label>
                    <input className="form-control" value={form.empresa ?? ''}
                      onChange={e => setForm(f => ({ ...f, empresa: e.target.value }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Telefone</label>
                    <input className="form-control" value={form.telefone ?? ''}
                      onChange={e => setForm(f => ({ ...f, telefone: e.target.value }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Origem *</label>
                    <select className="form-select" value={form.origem}
                      onChange={e => setForm(f => ({ ...f, origem: Number(e.target.value) }))}>
                      {ORIGENS.map(o => (
                        <option key={o.value} value={o.value}>{o.label}</option>
                      ))}
                    </select>
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Valor Estimado (R$)</label>
                    <input className="form-control" type="number" min="0" step="1000"
                      value={form.valorEstimado}
                      onChange={e => setForm(f => ({ ...f, valorEstimado: Number(e.target.value) }))} />
                  </div>
                  <div className="col-12">
                    <label className="form-label fw-medium">Notas</label>
                    <textarea className="form-control" rows={3} value={form.notas ?? ''}
                      onChange={e => setForm(f => ({ ...f, notas: e.target.value }))} />
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={closeModal}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!form.nome || !form.email || isWorking}
                  onClick={handleSave}>
                  {isWorking ? 'Salvando...' : 'Salvar'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
