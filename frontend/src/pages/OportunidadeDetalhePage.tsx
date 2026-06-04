import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ArrowLeft, PlusCircle, Phone, Users, Mail, CheckSquare, Pencil, Trash2 } from 'lucide-react'
import { oportunidadeApi } from '../api/oportunidadeApi'
import {
  atividadeApi,
  AtividadeDto,
  CriarAtividadeRequest,
  TipoAtividade,
  TIPO_LABEL,
} from '../api/atividadeApi'
import { StatusOportunidade } from '../api/oportunidadeApi'

const STATUS_BADGE: Record<number, string> = {
  1: 'bg-primary',
  2: 'bg-warning text-dark',
  3: 'bg-success',
  4: 'bg-danger',
  5: 'bg-secondary',
}

const TIPO_ICON: Record<number, React.ReactNode> = {
  1: <Phone    size={15} />,
  2: <Users    size={15} />,
  3: <Mail     size={15} />,
  4: <CheckSquare size={15} />,
}

const emptyForm = (oportunidadeId: string): CriarAtividadeRequest => ({
  oportunidadeId,
  tipo: TipoAtividade.Ligacao,
  descricao: '',
  dataHora: new Date().toISOString().slice(0, 16),
})

export default function OportunidadeDetalhePage() {
  const { id = '' } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const qc = useQueryClient()

  const [showModal, setShowModal] = useState(false)
  const [editAtividade, setEditAtividade] = useState<AtividadeDto | null>(null)
  const [form, setForm] = useState<CriarAtividadeRequest>(emptyForm(id))

  const { data: oportunidade, isLoading: loadingOp } = useQuery({
    queryKey: ['oportunidade', id],
    queryFn:  () => oportunidadeApi.obter(id),
    enabled:  !!id,
  })

  const { data: atividades = [], isLoading: loadingAts } = useQuery({
    queryKey: ['atividades', id],
    queryFn:  () => atividadeApi.listar(id),
    enabled:  !!id,
  })

  const invalidate = () => qc.invalidateQueries({ queryKey: ['atividades', id] })

  const criarMutation = useMutation({
    mutationFn: atividadeApi.criar,
    onSuccess: () => { invalidate(); closeModal() },
  })

  const atualizarMutation = useMutation({
    mutationFn: ({ atId, req }: { atId: string; req: { tipo: number; descricao: string; dataHora: string } }) =>
      atividadeApi.atualizar(atId, req),
    onSuccess: () => { invalidate(); closeModal() },
  })

  const excluirMutation = useMutation({
    mutationFn: atividadeApi.excluir,
    onSuccess: invalidate,
  })

  const concluirMutation = useMutation({
    mutationFn: atividadeApi.concluir,
    onSuccess: invalidate,
  })

  function openCriar() {
    setEditAtividade(null)
    setForm(emptyForm(id))
    setShowModal(true)
  }

  function openEditar(a: AtividadeDto) {
    setEditAtividade(a)
    setForm({
      oportunidadeId: id,
      tipo:      a.tipo,
      descricao: a.descricao,
      dataHora:  a.dataHora.slice(0, 16),
    })
    setShowModal(true)
  }

  function closeModal() {
    setShowModal(false)
    setEditAtividade(null)
    setForm(emptyForm(id))
  }

  function handleSalvar() {
    if (editAtividade) {
      atualizarMutation.mutate({
        atId: editAtividade.id,
        req: { tipo: form.tipo, descricao: form.descricao, dataHora: form.dataHora },
      })
    } else {
      criarMutation.mutate(form)
    }
  }

  const isPending = criarMutation.isPending || atualizarMutation.isPending

  if (loadingOp) return <div className="p-4 text-muted">Carregando...</div>
  if (!oportunidade) return <div className="p-4 text-danger">Oportunidade não encontrada.</div>

  return (
    <div>
      {/* Header */}
      <div className="d-flex align-items-center gap-3 mb-4">
        <button className="btn btn-outline-secondary btn-sm" onClick={() => navigate('/oportunidades')}>
          <ArrowLeft size={15} />
        </button>
        <div className="flex-grow-1">
          <h4 className="fw-bold mb-0">{oportunidade.titulo}</h4>
          <small className="text-muted">{oportunidade.leadNome ?? oportunidade.contatoNome ?? '—'}</small>
        </div>
        <div className="d-flex align-items-center gap-2">
          <span className="fw-semibold">
            R$ {oportunidade.valor.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}
          </span>
          <span className="badge bg-secondary">{oportunidade.probabilidade}%</span>
          <span className={`badge ${STATUS_BADGE[oportunidade.status] ?? 'bg-secondary'}`}>
            {oportunidade.statusNome}
          </span>
        </div>
      </div>

      {/* Atividades */}
      <div className="d-flex align-items-center justify-content-between mb-3">
        <h5 className="fw-semibold mb-0">Atividades</h5>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2"
          onClick={openCriar}>
          <PlusCircle size={14} /> Nova Atividade
        </button>
      </div>

      {loadingAts ? (
        <div className="text-muted py-3">Carregando atividades...</div>
      ) : atividades.length === 0 ? (
        <div className="card card-kpi p-4 text-center text-muted">
          Nenhuma atividade registrada.
        </div>
      ) : (
        <div className="card card-kpi p-0">
          <ul className="list-group list-group-flush">
            {atividades.map(a => (
              <li key={a.id}
                className={`list-group-item d-flex align-items-start gap-3 py-3${a.concluida ? ' opacity-50' : ''}`}>
                {/* Tipo icon */}
                <div className="mt-1 text-primary">{TIPO_ICON[a.tipo]}</div>

                {/* Content */}
                <div className="flex-grow-1">
                  <div className="d-flex align-items-center gap-2 mb-1">
                    <span className="badge bg-light text-dark border">{TIPO_LABEL[a.tipo]}</span>
                    <small className="text-muted">
                      {new Date(a.dataHora).toLocaleString('pt-BR', {
                        day: '2-digit', month: '2-digit', year: 'numeric',
                        hour: '2-digit', minute: '2-digit',
                      })}
                    </small>
                    {a.concluida && <span className="badge bg-success">Concluída</span>}
                  </div>
                  <p className={`mb-0 small${a.concluida ? ' text-decoration-line-through' : ''}`}>
                    {a.descricao}
                  </p>
                </div>

                {/* Actions */}
                <div className="d-flex gap-1">
                  <button className={`btn btn-sm ${a.concluida ? 'btn-success' : 'btn-outline-success'}`}
                    title={a.concluida ? 'Marcar como pendente' : 'Marcar como concluída'}
                    onClick={() => concluirMutation.mutate(a.id)}>
                    <CheckSquare size={13} />
                  </button>
                  <button className="btn btn-sm btn-outline-primary"
                    onClick={() => openEditar(a)}>
                    <Pencil size={13} />
                  </button>
                  <button className="btn btn-sm btn-outline-danger"
                    onClick={() => {
                      if (confirm('Excluir atividade?')) excluirMutation.mutate(a.id)
                    }}>
                    <Trash2 size={13} />
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Modal */}
      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">
                  {editAtividade ? 'Editar Atividade' : 'Nova Atividade'}
                </h5>
                <button className="btn-close" onClick={closeModal} />
              </div>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label fw-medium">Tipo *</label>
                  <select className="form-select" value={form.tipo}
                    onChange={e => setForm(f => ({ ...f, tipo: Number(e.target.value) }))}>
                    {Object.entries(TIPO_LABEL).map(([v, label]) => (
                      <option key={v} value={v}>{label}</option>
                    ))}
                  </select>
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Data e Hora *</label>
                  <input className="form-control" type="datetime-local"
                    value={form.dataHora}
                    onChange={e => setForm(f => ({ ...f, dataHora: e.target.value }))} />
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Descrição *</label>
                  <textarea className="form-control" rows={3}
                    value={form.descricao}
                    onChange={e => setForm(f => ({ ...f, descricao: e.target.value }))} />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={closeModal}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!form.descricao || isPending}
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
