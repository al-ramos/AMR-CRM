import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { PlusCircle } from 'lucide-react'
import { oportunidadeApi, CriarOportunidadeRequest, StatusOportunidade } from '../api/oportunidadeApi'
import { contatoApi } from '../api/contatoApi'
import { leadApi } from '../api/leadApi'

const STATUS_BADGE: Record<number, string> = {
  1: 'badge-aberta',
  2: 'badge-andamento',
  3: 'badge-ganha',
  4: 'badge-perdida',
  5: 'badge-cancelada',
}

const fmtBRL = (v: number) =>
  v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 })

type Origem = 'lead' | 'contato'

const EMPTY: CriarOportunidadeRequest = { titulo: '', valor: 0, probabilidade: 50 }

export default function OportunidadesPage() {
  const qc = useQueryClient()
  const [showModal, setShowModal] = useState(false)
  const [origem, setOrigem]       = useState<Origem>('lead')
  const [form, setForm]           = useState<CriarOportunidadeRequest>(EMPTY)

  const { data: oportunidades = [], isLoading } = useQuery({
    queryKey: ['oportunidades'],
    queryFn: oportunidadeApi.listar,
  })
  const { data: leads = [] } = useQuery({
    queryKey: ['leads'],
    queryFn:  () => leadApi.listar(),
  })
  const { data: contatos = [] } = useQuery({
    queryKey: ['contatos'],
    queryFn: contatoApi.listar,
  })

  const criarMutation = useMutation({
    mutationFn: oportunidadeApi.criar,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['oportunidades'] })
      setShowModal(false)
      setForm(EMPTY)
    },
  })

  const avancarMutation = useMutation({
    mutationFn: ({ id, novoStatus }: { id: string; novoStatus: number }) =>
      oportunidadeApi.avancarStatus(id, novoStatus),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['oportunidades'] }),
  })

  // pipeline ponderado = Σ (valor * probabilidade/100) para op. em aberto / andamento
  const pipeline = oportunidades
    .filter(o => o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento)
    .reduce((s, o) => s + o.valor * (o.probabilidade / 100), 0)

  const handleSave = () => {
    const req: CriarOportunidadeRequest = {
      ...form,
      leadId:    origem === 'lead'    ? form.leadId    : undefined,
      contatoId: origem === 'contato' ? form.contatoId : undefined,
    }
    criarMutation.mutate(req)
  }

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h4 className="fw-bold mb-0">Oportunidades</h4>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2"
          onClick={() => setShowModal(true)}>
          <PlusCircle size={15} /> Nova Oportunidade
        </button>
      </div>

      <div className="card card-kpi">
        {isLoading ? (
          <div className="p-4 text-center text-muted">Carregando...</div>
        ) : (
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table-light">
                <tr>
                  <th>Título</th>
                  <th>Lead / Contato</th>
                  <th>Valor</th>
                  <th>Prob.</th>
                  <th>Previsão</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {oportunidades.map(o => (
                  <tr key={o.id}>
                    <td className="fw-medium">{o.titulo}</td>
                    <td className="small text-muted">
                      {o.leadNome
                        ? <><span className="badge bg-primary me-1" style={{fontSize:'0.65rem'}}>L</span>{o.leadNome}</>
                        : o.contatoNome
                        ? <><span className="badge bg-secondary me-1" style={{fontSize:'0.65rem'}}>C</span>{o.contatoNome}</>
                        : '—'}
                    </td>
                    <td className="small">{fmtBRL(o.valor)}</td>
                    <td>
                      <div className="d-flex align-items-center gap-1">
                        <div className="progress" style={{ width: 50, height: 6 }}>
                          <div className="progress-bar"
                            style={{ width: `${o.probabilidade}%`,
                              background: o.probabilidade >= 70 ? '#198754' : o.probabilidade >= 40 ? '#fd7e14' : '#dc3545' }} />
                        </div>
                        <small className="text-muted">{o.probabilidade}%</small>
                      </div>
                    </td>
                    <td className="small">
                      {o.previsaoFechamento
                        ? new Date(o.previsaoFechamento).toLocaleDateString('pt-BR')
                        : '—'}
                    </td>
                    <td>
                      <span className={`badge ${STATUS_BADGE[o.status] ?? 'bg-secondary'}`}>
                        {o.statusNome}
                      </span>
                    </td>
                    <td>
                      <div className="d-flex gap-1">
                        {o.status === StatusOportunidade.Aberta && (
                          <button className="btn btn-outline-warning btn-sm"
                            style={{ fontSize: '0.75rem' }}
                            onClick={() => avancarMutation.mutate({ id: o.id, novoStatus: StatusOportunidade.EmAndamento })}>
                            Iniciar
                          </button>
                        )}
                        {(o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento) && (
                          <>
                            <button className="btn btn-outline-success btn-sm"
                              style={{ fontSize: '0.75rem' }}
                              onClick={() => avancarMutation.mutate({ id: o.id, novoStatus: StatusOportunidade.Ganha })}>
                              Ganhar
                            </button>
                            <button className="btn btn-outline-danger btn-sm"
                              style={{ fontSize: '0.75rem' }}
                              onClick={() => avancarMutation.mutate({ id: o.id, novoStatus: StatusOportunidade.Perdida })}>
                              Perder
                            </button>
                          </>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
                {oportunidades.length === 0 && (
                  <tr>
                    <td colSpan={7} className="text-center text-muted py-4">
                      Nenhuma oportunidade cadastrada.
                    </td>
                  </tr>
                )}
              </tbody>
              {oportunidades.length > 0 && (
                <tfoot className="table-light">
                  <tr>
                    <td colSpan={2} className="fw-semibold small">Pipeline ponderado</td>
                    <td className="fw-bold text-primary small" colSpan={2}>
                      {fmtBRL(pipeline)}
                    </td>
                    <td colSpan={3}></td>
                  </tr>
                </tfoot>
              )}
            </table>
          </div>
        )}
      </div>

      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog modal-lg">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Nova Oportunidade</h5>
                <button className="btn-close" onClick={() => setShowModal(false)} />
              </div>
              <div className="modal-body">
                <div className="row g-3">
                  <div className="col-12">
                    <label className="form-label fw-medium">Título *</label>
                    <input className="form-control" value={form.titulo}
                      onChange={e => setForm(f => ({ ...f, titulo: e.target.value }))} />
                  </div>

                  <div className="col-md-6">
                    <label className="form-label fw-medium">Valor (R$) *</label>
                    <input className="form-control" type="number" min="0" value={form.valor}
                      onChange={e => setForm(f => ({ ...f, valor: Number(e.target.value) }))} />
                  </div>
                  <div className="col-md-6">
                    <label className="form-label fw-medium">Probabilidade (%)</label>
                    <input className="form-control" type="number" min="0" max="100"
                      value={form.probabilidade ?? 50}
                      onChange={e => setForm(f => ({ ...f, probabilidade: Number(e.target.value) }))} />
                  </div>

                  {/* toggle Lead vs Contato */}
                  <div className="col-12">
                    <div className="btn-group w-100">
                      <button className={`btn btn-sm ${origem === 'lead' ? 'btn-primary' : 'btn-outline-secondary'}`}
                        onClick={() => { setOrigem('lead'); setForm(f => ({ ...f, contatoId: undefined })) }}>
                        Vincular a Lead
                      </button>
                      <button className={`btn btn-sm ${origem === 'contato' ? 'btn-primary' : 'btn-outline-secondary'}`}
                        onClick={() => { setOrigem('contato'); setForm(f => ({ ...f, leadId: undefined })) }}>
                        Vincular a Contato
                      </button>
                    </div>
                  </div>

                  {origem === 'lead' && (
                    <div className="col-12">
                      <label className="form-label fw-medium">Lead *</label>
                      <select className="form-select" value={form.leadId ?? ''}
                        onChange={e => setForm(f => ({ ...f, leadId: e.target.value }))}>
                        <option value="">Selecionar...</option>
                        {leads.filter(l => l.status < 4).map(l => (
                          <option key={l.id} value={l.id}>
                            {l.nome} {l.empresa ? `— ${l.empresa}` : ''}
                          </option>
                        ))}
                      </select>
                    </div>
                  )}

                  {origem === 'contato' && (
                    <div className="col-12">
                      <label className="form-label fw-medium">Contato *</label>
                      <select className="form-select" value={form.contatoId ?? ''}
                        onChange={e => setForm(f => ({ ...f, contatoId: e.target.value }))}>
                        <option value="">Selecionar...</option>
                        {contatos.filter(c => c.status === 1).map(c => (
                          <option key={c.id} value={c.id}>
                            {c.nome} {c.empresa ? `— ${c.empresa}` : ''}
                          </option>
                        ))}
                      </select>
                    </div>
                  )}

                  <div className="col-md-6">
                    <label className="form-label fw-medium">Previsão de Fechamento</label>
                    <input className="form-control" type="date"
                      value={form.previsaoFechamento ?? ''}
                      onChange={e => setForm(f => ({ ...f, previsaoFechamento: e.target.value || undefined }))} />
                  </div>
                  <div className="col-12">
                    <label className="form-label fw-medium">Descrição</label>
                    <textarea className="form-control" rows={2} value={form.descricao ?? ''}
                      onChange={e => setForm(f => ({ ...f, descricao: e.target.value }))} />
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!form.titulo || (!form.leadId && !form.contatoId) || criarMutation.isPending}
                  onClick={handleSave}>
                  {criarMutation.isPending ? 'Salvando...' : 'Salvar'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
