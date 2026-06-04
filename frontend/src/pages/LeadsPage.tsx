import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { PlusCircle, Pencil, Trash2 } from 'lucide-react'
import {
  leadApi, StatusLead, OrigemLead, ORIGEM_LABEL,
  type LeadDto, type CriarLeadRequest,
} from '../api/leadApi'

const STATUS_CFG: Record<number, { label: string; cls: string }> = {
  [StatusLead.Novo]:        { label: 'Novo',        cls: 'bg-secondary' },
  [StatusLead.Qualificado]: { label: 'Qualificado', cls: 'bg-info text-dark' },
  [StatusLead.Proposta]:    { label: 'Proposta',    cls: 'bg-warning text-dark' },
  [StatusLead.Ganho]:       { label: 'Ganho',       cls: 'bg-success' },
  [StatusLead.Perdido]:     { label: 'Perdido',     cls: 'bg-danger' },
}

const EMPTY: CriarLeadRequest = { nome: '', email: '', origem: OrigemLead.Site, valorEstimado: 0 }

const fmt = (v: number) =>
  v.toLocaleString('pt-BR', { minimumFractionDigits: 0, maximumFractionDigits: 2 })

export default function LeadsPage() {
  const qc = useQueryClient()
  const [filtro, setFiltro]           = useState<number | ''>('')
  const [showModal, setShowModal]     = useState(false)
  const [editando, setEditando]       = useState<LeadDto | null>(null)
  const [form, setForm]               = useState<CriarLeadRequest>(EMPTY)

  const { data: leads = [], isLoading } = useQuery({
    queryKey: ['leads'],
    queryFn: leadApi.listar,
  })

  const invalidate = () => {
    qc.invalidateQueries({ queryKey: ['leads'] })
    qc.invalidateQueries({ queryKey: ['dashboard'] })
  }

  const criarMutation = useMutation({
    mutationFn: leadApi.criar,
    onSuccess: () => { invalidate(); fecharModal() },
  })
  const atualizarMutation = useMutation({
    mutationFn: ({ id, req }: { id: string; req: CriarLeadRequest }) => leadApi.atualizar(id, req),
    onSuccess: () => { invalidate(); fecharModal() },
  })
  const excluirMutation = useMutation({
    mutationFn: leadApi.excluir,
    onSuccess: invalidate,
  })
  const statusMutation = useMutation({
    mutationFn: ({ id, acao }: { id: string; acao: string }) =>
      acao === 'qualificar' ? leadApi.qualificar(id)
        : acao === 'proposta' ? leadApi.proposta(id)
        : acao === 'ganhar'   ? leadApi.ganhar(id)
        : leadApi.perder(id),
    onSuccess: invalidate,
  })

  function abrirCriar()             { setEditando(null); setForm(EMPTY); setShowModal(true) }
  function abrirEditar(l: LeadDto)  {
    setEditando(l)
    setForm({ nome: l.nome, email: l.email, origem: l.origem, valorEstimado: l.valorEstimado,
              telefone: l.telefone, empresa: l.empresa, notas: l.notas })
    setShowModal(true)
  }
  function fecharModal()            { setShowModal(false); setEditando(null); setForm(EMPTY) }

  function salvar() {
    if (editando) atualizarMutation.mutate({ id: editando.id, req: form })
    else criarMutation.mutate(form)
  }

  const filtrados = filtro === '' ? leads : leads.filter(l => l.status === filtro)
  const totalEstimado = filtrados.reduce((s, l) => s + l.valorEstimado, 0)
  const isPending = criarMutation.isPending || atualizarMutation.isPending

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h4 className="fw-bold mb-0">Leads</h4>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2" onClick={abrirCriar}>
          <PlusCircle size={15} /> Novo Lead
        </button>
      </div>

      {/* Filtro por status */}
      <div className="d-flex gap-2 mb-3 flex-wrap">
        <button className={`btn btn-sm ${filtro === '' ? 'btn-dark' : 'btn-outline-secondary'}`}
          onClick={() => setFiltro('')}>Todos ({leads.length})</button>
        {Object.entries(STATUS_CFG).map(([st, cfg]) => {
          const count = leads.filter(l => l.status === Number(st)).length
          return (
            <button key={st}
              className={`btn btn-sm ${filtro === Number(st) ? cfg.cls : 'btn-outline-secondary'}`}
              onClick={() => setFiltro(filtro === Number(st) ? '' : Number(st))}>
              {cfg.label} ({count})
            </button>
          )
        })}
      </div>

      <div className="card card-kpi">
        {isLoading ? (
          <div className="p-4 text-center">
            <div className="spinner-border spinner-border-sm text-primary" />
          </div>
        ) : (
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>Nome</th>
                  <th>Empresa</th>
                  <th>Origem</th>
                  <th>Valor Est.</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {filtrados.map(l => (
                  <tr key={l.id}>
                    <td>
                      <div className="fw-medium">{l.nome}</div>
                      <small className="text-muted">{l.email}</small>
                    </td>
                    <td className="small text-muted">{l.empresa ?? '—'}</td>
                    <td><span className="badge bg-light text-dark border">{ORIGEM_LABEL[l.origem]}</span></td>
                    <td>R$ {fmt(l.valorEstimado)}</td>
                    <td>
                      <span className={`badge ${STATUS_CFG[l.status]?.cls ?? 'bg-secondary'}`}>
                        {STATUS_CFG[l.status]?.label ?? l.statusNome}
                      </span>
                    </td>
                    <td>
                      <div className="d-flex gap-1 flex-wrap">
                        {l.status === StatusLead.Novo && (
                          <button className="btn btn-outline-info btn-sm"
                            onClick={() => statusMutation.mutate({ id: l.id, acao: 'qualificar' })}>
                            Qualificar
                          </button>
                        )}
                        {l.status === StatusLead.Qualificado && (
                          <button className="btn btn-outline-warning btn-sm"
                            onClick={() => statusMutation.mutate({ id: l.id, acao: 'proposta' })}>
                            Proposta
                          </button>
                        )}
                        {(l.status === StatusLead.Proposta || l.status === StatusLead.Qualificado) && (
                          <>
                            <button className="btn btn-outline-success btn-sm"
                              onClick={() => statusMutation.mutate({ id: l.id, acao: 'ganhar' })}>
                              Ganhar
                            </button>
                            <button className="btn btn-outline-danger btn-sm"
                              onClick={() => statusMutation.mutate({ id: l.id, acao: 'perder' })}>
                              Perder
                            </button>
                          </>
                        )}
                        <button className="btn btn-outline-secondary btn-sm"
                          onClick={() => abrirEditar(l)}>
                          <Pencil size={13} />
                        </button>
                        <button className="btn btn-outline-danger btn-sm"
                          onClick={() => { if (confirm(`Excluir "${l.nome}"?`)) excluirMutation.mutate(l.id) }}>
                          <Trash2 size={13} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {filtrados.length === 0 && (
                  <tr>
                    <td colSpan={6} className="text-center text-muted py-4">
                      {filtro !== '' ? 'Nenhum lead com este status.' : 'Nenhum lead cadastrado.'}
                    </td>
                  </tr>
                )}
              </tbody>
              {filtrados.length > 0 && (
                <tfoot className="table-light">
                  <tr>
                    <td colSpan={3} className="fw-semibold">
                      Total ({filtrados.length} lead{filtrados.length !== 1 ? 's' : ''})
                    </td>
                    <td className="fw-bold">R$ {fmt(totalEstimado)}</td>
                    <td colSpan={2} />
                  </tr>
                </tfoot>
              )}
            </table>
          </div>
        )}
      </div>

      {/* Modal criar/editar */}
      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">{editando ? 'Editar Lead' : 'Novo Lead'}</h5>
                <button className="btn-close" onClick={fecharModal} />
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
                      onChange={e => setForm(f => ({ ...f, empresa: e.target.value || undefined }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Telefone</label>
                    <input className="form-control" value={form.telefone ?? ''}
                      onChange={e => setForm(f => ({ ...f, telefone: e.target.value || undefined }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Origem *</label>
                    <select className="form-select" value={form.origem}
                      onChange={e => setForm(f => ({ ...f, origem: Number(e.target.value) }))}>
                      {Object.entries(ORIGEM_LABEL).map(([k, v]) => (
                        <option key={k} value={k}>{v}</option>
                      ))}
                    </select>
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Valor Estimado (R$) *</label>
                    <input className="form-control" type="number" min="0" step="0.01"
                      value={form.valorEstimado}
                      onChange={e => setForm(f => ({ ...f, valorEstimado: Number(e.target.value) }))} />
                  </div>
                  <div className="col-12">
                    <label className="form-label fw-medium">Notas</label>
                    <textarea className="form-control" rows={2} value={form.notas ?? ''}
                      onChange={e => setForm(f => ({ ...f, notas: e.target.value || undefined }))} />
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={fecharModal}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!form.nome || !form.email || isPending}
                  onClick={salvar}>
                  {isPending ? 'Salvando...' : 'Salvar'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
