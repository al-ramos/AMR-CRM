import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { PlusCircle, Pencil, Trash2, Eye } from 'lucide-react'
import {
  oportunidadeApi,
  OportunidadeDto,
  CriarOportunidadeRequest,
  AtualizarOportunidadeRequest,
  StatusOportunidade,
} from '../api/oportunidadeApi'
import { leadApi } from '../api/leadApi'

const STATUS_BADGE: Record<number, string> = {
  1: 'bg-primary',
  2: 'bg-warning text-dark',
  3: 'bg-success',
  4: 'bg-danger',
  5: 'bg-secondary',
}

const STATUS_LABEL: Record<number, string> = {
  1: 'Aberta',
  2: 'Em Andamento',
  3: 'Ganha',
  4: 'Perdida',
  5: 'Cancelada',
}

type ModalMode = 'criar' | 'editar'

const emptyForm = (): CriarOportunidadeRequest => ({
  titulo: '', valor: 0, leadId: '', probabilidade: 0,
})

export default function OportunidadesPage() {
  const qc = useQueryClient()
  const navigate = useNavigate()
  const [showModal, setShowModal] = useState(false)
  const [modalMode, setModalMode] = useState<ModalMode>('criar')
  const [editId, setEditId] = useState<string | null>(null)
  const [form, setForm] = useState<CriarOportunidadeRequest>(emptyForm())

  const { data: oportunidades = [], isLoading } = useQuery({
    queryKey: ['oportunidades'],
    queryFn: oportunidadeApi.listar,
  })
  const { data: leads = [] } = useQuery({
    queryKey: ['leads'],
    queryFn: leadApi.listar,
  })

  const invalidate = () => qc.invalidateQueries({ queryKey: ['oportunidades'] })

  const criarMutation = useMutation({
    mutationFn: oportunidadeApi.criar,
    onSuccess: () => { invalidate(); closeModal() },
  })

  const atualizarMutation = useMutation({
    mutationFn: ({ id, req }: { id: string; req: AtualizarOportunidadeRequest }) =>
      oportunidadeApi.atualizar(id, req),
    onSuccess: () => { invalidate(); closeModal() },
  })

  const excluirMutation = useMutation({
    mutationFn: oportunidadeApi.excluir,
    onSuccess: invalidate,
  })

  const statusMutation = useMutation({
    mutationFn: ({ id, action }: { id: string; action: string }) => {
      if (action === 'iniciar')  return oportunidadeApi.iniciar(id)
      if (action === 'ganhar')   return oportunidadeApi.ganhar(id)
      if (action === 'perder')   return oportunidadeApi.perder(id)
      return oportunidadeApi.cancelar(id)
    },
    onSuccess: invalidate,
  })

  function openCriar() {
    setModalMode('criar')
    setForm(emptyForm())
    setEditId(null)
    setShowModal(true)
  }

  function openEditar(o: OportunidadeDto) {
    setModalMode('editar')
    setEditId(o.id)
    setForm({
      titulo: o.titulo,
      valor: o.valor,
      leadId: o.leadId ?? '',
      probabilidade: o.probabilidade,
      descricao: o.descricao,
      previsaoFechamento: o.previsaoFechamento
        ? o.previsaoFechamento.split('T')[0]
        : undefined,
    })
    setShowModal(true)
  }

  function closeModal() {
    setShowModal(false)
    setForm(emptyForm())
    setEditId(null)
  }

  function handleSalvar() {
    const payload = {
      ...form,
      leadId: form.leadId || undefined,
      previsaoFechamento: form.previsaoFechamento || undefined,
    }
    if (modalMode === 'criar') {
      criarMutation.mutate(payload)
    } else if (editId) {
      atualizarMutation.mutate({
        id: editId,
        req: {
          titulo: form.titulo,
          valor: form.valor,
          probabilidade: form.probabilidade,
          descricao: form.descricao,
          previsaoFechamento: form.previsaoFechamento || undefined,
        },
      })
    }
  }

  const totalPipeline = oportunidades
    .filter(o => o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento)
    .reduce((s, o) => s + o.valor, 0)

  const isPending = criarMutation.isPending || atualizarMutation.isPending
  const canSalvar = !!form.titulo && form.valor >= 0 && !isPending

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h4 className="fw-bold mb-0">Oportunidades</h4>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2"
          onClick={openCriar}>
          <PlusCircle size={15} /> Nova Oportunidade
        </button>
      </div>

      <div className="card card-kpi">
        {isLoading ? (
          <div className="p-4 text-center text-muted">Carregando...</div>
        ) : (
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>Título</th>
                  <th>Lead</th>
                  <th>Valor</th>
                  <th>%</th>
                  <th>Previsão</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {oportunidades.map(o => (
                  <tr key={o.id}>
                    <td className="fw-medium">{o.titulo}</td>
                    <td className="small text-muted">{o.leadNome ?? o.contatoNome ?? '—'}</td>
                    <td>R$ {o.valor.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}</td>
                    <td className="small">{o.probabilidade}%</td>
                    <td className="small">
                      {o.previsaoFechamento
                        ? new Date(o.previsaoFechamento).toLocaleDateString('pt-BR')
                        : '—'}
                    </td>
                    <td>
                      <span className={`badge ${STATUS_BADGE[o.status] ?? 'bg-secondary'}`}>
                        {STATUS_LABEL[o.status] ?? o.statusNome}
                      </span>
                    </td>
                    <td>
                      <div className="d-flex gap-1 flex-wrap">
                        {o.status === StatusOportunidade.Aberta && (
                          <button className="btn btn-outline-warning btn-sm"
                            onClick={() => statusMutation.mutate({ id: o.id, action: 'iniciar' })}>
                            Iniciar
                          </button>
                        )}
                        {(o.status === StatusOportunidade.Aberta ||
                          o.status === StatusOportunidade.EmAndamento) && (
                          <>
                            <button className="btn btn-outline-success btn-sm"
                              onClick={() => statusMutation.mutate({ id: o.id, action: 'ganhar' })}>
                              Ganhar
                            </button>
                            <button className="btn btn-outline-danger btn-sm"
                              onClick={() => statusMutation.mutate({ id: o.id, action: 'perder' })}>
                              Perder
                            </button>
                            <button className="btn btn-outline-secondary btn-sm"
                              onClick={() => statusMutation.mutate({ id: o.id, action: 'cancelar' })}>
                              Cancelar
                            </button>
                          </>
                        )}
                        <button className="btn btn-outline-info btn-sm"
                          title="Ver atividades"
                          onClick={() => navigate(`/oportunidades/${o.id}`)}>
                          <Eye size={13} />
                        </button>
                        <button className="btn btn-outline-primary btn-sm"
                          onClick={() => openEditar(o)}>
                          <Pencil size={13} />
                        </button>
                        <button className="btn btn-outline-danger btn-sm"
                          onClick={() => {
                            if (confirm(`Excluir "${o.titulo}"?`))
                              excluirMutation.mutate(o.id)
                          }}>
                          <Trash2 size={13} />
                        </button>
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
                    <td colSpan={2} className="fw-semibold">Pipeline Ativo (Abertas + Em Andamento)</td>
                    <td className="fw-bold">
                      R$ {totalPipeline.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}
                    </td>
                    <td colSpan={4} />
                  </tr>
                </tfoot>
              )}
            </table>
          </div>
        )}
      </div>

      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  {modalMode === 'criar' ? 'Nova Oportunidade' : 'Editar Oportunidade'}
                </h5>
                <button className="btn-close" onClick={closeModal} />
              </div>
              <div className="modal-body">
                {modalMode === 'criar' && (
                  <div className="mb-3">
                    <label className="form-label fw-medium">Lead</label>
                    <select className="form-select" value={form.leadId ?? ''}
                      onChange={e => setForm(f => ({ ...f, leadId: e.target.value }))}>
                      <option value="">Selecionar lead (opcional)...</option>
                      {leads.map(l => (
                        <option key={l.id} value={l.id}>
                          {l.nome}{l.empresa ? ` — ${l.empresa}` : ''}
                        </option>
                      ))}
                    </select>
                  </div>
                )}
                <div className="mb-3">
                  <label className="form-label fw-medium">Título *</label>
                  <input className="form-control" value={form.titulo}
                    onChange={e => setForm(f => ({ ...f, titulo: e.target.value }))} />
                </div>
                <div className="row g-3 mb-3">
                  <div className="col">
                    <label className="form-label fw-medium">Valor (R$) *</label>
                    <input className="form-control" type="number" min="0" value={form.valor}
                      onChange={e => setForm(f => ({ ...f, valor: Number(e.target.value) }))} />
                  </div>
                  <div className="col">
                    <label className="form-label fw-medium">Probabilidade (%)</label>
                    <input className="form-control" type="number" min="0" max="100"
                      value={form.probabilidade ?? 0}
                      onChange={e => setForm(f => ({ ...f, probabilidade: Number(e.target.value) }))} />
                  </div>
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Previsão de Fechamento</label>
                  <input className="form-control" type="date"
                    value={form.previsaoFechamento ?? ''}
                    onChange={e => setForm(f => ({ ...f, previsaoFechamento: e.target.value || undefined }))} />
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Descrição</label>
                  <textarea className="form-control" rows={3} value={form.descricao ?? ''}
                    onChange={e => setForm(f => ({ ...f, descricao: e.target.value }))} />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={closeModal}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!canSalvar}
                  onClick={handleSalvar}>
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
