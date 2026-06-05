import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { PlusCircle, Pencil, Trash2, Bell } from 'lucide-react'
import {
  atividadeApi, AtividadeDto, CriarAtividadeRequest, AtualizarAtividadeRequest,
  TipoAtividade, StatusAtividade, TIPOS, STATUS_ATIVIDADE,
} from '../api/atividadeApi'

// ── badges ───────────────────────────────────────────────────────────────────
const TIPO_LABEL: Record<number, string> = {
  1: 'Ligação', 2: 'E-mail', 3: 'Reunião', 4: 'Tarefa', 5: 'Visita',
}
const TIPO_CLASS: Record<number, string> = {
  1: 'bg-primary',
  2: 'bg-info text-dark',
  3: 'bg-warning text-dark',
  4: 'bg-secondary',
  5: 'bg-success',
}
const STATUS_CLASS: Record<number, string> = {
  1: 'bg-warning text-dark',
  2: 'bg-success',
  3: 'bg-secondary',
}

// ── helpers ──────────────────────────────────────────────────────────────────
const fmtDate = (iso: string) =>
  new Date(iso).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' })

const toInputDate = (iso: string) => iso.slice(0, 10)

const EMPTY_CRIAR: CriarAtividadeRequest = {
  titulo: '',
  tipo: TipoAtividade.Ligacao,
  dataPrevista: new Date().toISOString().slice(0, 10),
}

interface EditForm {
  titulo: string
  tipo: number
  dataPrevista: string
  status: number
  observacao: string
}

const emptyEdit = (): EditForm => ({
  titulo: '',
  tipo: TipoAtividade.Ligacao,
  dataPrevista: new Date().toISOString().slice(0, 10),
  status: StatusAtividade.Pendente,
  observacao: '',
})

export default function AtividadesPage() {
  const qc = useQueryClient()

  const [filtroStatus, setFiltroStatus] = useState<number | undefined>(undefined)
  const [filtroTipo,   setFiltroTipo]   = useState<number | undefined>(undefined)
  const [showModal,    setShowModal]    = useState(false)
  const [editing,      setEditing]      = useState<AtividadeDto | null>(null)
  const [criarForm,    setCriarForm]    = useState<CriarAtividadeRequest>(EMPTY_CRIAR)
  const [editForm,     setEditForm]     = useState<EditForm>(emptyEdit())

  const { data: atividades = [], isLoading } = useQuery({
    queryKey: ['atividades', filtroStatus, filtroTipo],
    queryFn:  () => atividadeApi.listar(filtroStatus, filtroTipo),
  })

  const criarMut = useMutation({
    mutationFn: atividadeApi.criar,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['atividades'] }); closeModal() },
  })
  const atualizarMut = useMutation({
    mutationFn: ({ id, req }: { id: string; req: AtualizarAtividadeRequest }) =>
      atividadeApi.atualizar(id, req),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['atividades'] }); closeModal() },
  })
  const excluirMut = useMutation({
    mutationFn: atividadeApi.excluir,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['atividades'] }),
  })

  const openCreate = () => {
    setEditing(null)
    setCriarForm(EMPTY_CRIAR)
    setShowModal(true)
  }
  const openEdit = (a: AtividadeDto) => {
    setEditing(a)
    setEditForm({
      titulo:       a.titulo,
      tipo:         a.tipo,
      dataPrevista: toInputDate(a.dataPrevista),
      status:       a.status,
      observacao:   a.observacao ?? '',
    })
    setShowModal(true)
  }
  const closeModal = () => { setShowModal(false); setEditing(null) }

  const handleSave = () => {
    if (editing) {
      atualizarMut.mutate({
        id:  editing.id,
        req: {
          titulo:       editForm.titulo,
          tipo:         editForm.tipo,
          dataPrevista: new Date(editForm.dataPrevista).toISOString(),
          status:       editForm.status,
          observacao:   editForm.observacao || undefined,
        },
      })
    } else {
      criarMut.mutate({
        ...criarForm,
        dataPrevista: new Date(criarForm.dataPrevista).toISOString(),
      })
    }
  }

  const isWorking  = criarMut.isPending || atualizarMut.isPending
  const vencidasCt = atividades.filter(a => a.vencida).length

  return (
    <div>
      {/* ── cabeçalho ── */}
      <div className="d-flex align-items-center justify-content-between mb-3">
        <div className="d-flex align-items-center gap-2">
          <h4 className="fw-bold mb-0">Atividades</h4>
          {vencidasCt > 0 && (
            <span className="badge bg-danger d-flex align-items-center gap-1">
              <Bell size={12} /> {vencidasCt} vencida{vencidasCt > 1 ? 's' : ''}
            </span>
          )}
        </div>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2"
          onClick={openCreate}>
          <PlusCircle size={15} /> Nova Atividade
        </button>
      </div>

      {/* ── filtros ── */}
      <div className="d-flex gap-3 flex-wrap mb-3">
        <div className="d-flex gap-1 flex-wrap align-items-center">
          <span className="text-muted small me-1">Status:</span>
          {[undefined, 1, 2, 3].map(s => (
            <button key={s ?? 'all'}
              className={`btn btn-sm ${filtroStatus === s
                ? s ? STATUS_CLASS[s] : 'btn-dark'
                : 'btn-outline-secondary'}`}
              onClick={() => setFiltroStatus(s)}>
              {s ? STATUS_ATIVIDADE.find(x => x.value === s)?.label : 'Todos'}
            </button>
          ))}
        </div>
        <div className="d-flex gap-1 flex-wrap align-items-center">
          <span className="text-muted small me-1">Tipo:</span>
          {[undefined, 1, 2, 3, 4, 5].map(t => (
            <button key={t ?? 'all'}
              className={`btn btn-sm ${filtroTipo === t
                ? t ? TIPO_CLASS[t].replace('text-dark', '').trim() : 'btn-dark'
                : 'btn-outline-secondary'}`}
              onClick={() => setFiltroTipo(t)}>
              {t ? TIPO_LABEL[t] : 'Todos'}
            </button>
          ))}
        </div>
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
                  <th>Título</th>
                  <th>Tipo</th>
                  <th>Data Prevista</th>
                  <th>Status</th>
                  <th>Vinculado a</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {atividades.map(a => (
                  <tr key={a.id} style={{ opacity: a.status === StatusAtividade.Cancelada ? 0.55 : 1 }}>
                    <td>
                      <div className="fw-medium">{a.titulo}</div>
                      {a.observacao && (
                        <small className="text-muted">{a.observacao}</small>
                      )}
                    </td>
                    <td>
                      <span className={`badge ${TIPO_CLASS[a.tipo]}`}>
                        {TIPO_LABEL[a.tipo]}
                      </span>
                    </td>
                    <td className="small">
                      <div>{fmtDate(a.dataPrevista)}</div>
                      {a.vencida && (
                        <span className="badge bg-danger" style={{ fontSize: '0.65rem' }}>
                          Vencida
                        </span>
                      )}
                    </td>
                    <td>
                      <span className={`badge ${STATUS_CLASS[a.status]}`}>
                        {a.statusNome}
                      </span>
                    </td>
                    <td className="small">
                      {a.leadNome && (
                        <div><span className="text-muted">Lead:</span> {a.leadNome}</div>
                      )}
                      {a.oportunidadeTitulo && (
                        <div><span className="text-muted">Oport.:</span> {a.oportunidadeTitulo}</div>
                      )}
                      {!a.leadNome && !a.oportunidadeTitulo && '—'}
                    </td>
                    <td>
                      <div className="d-flex gap-1">
                        <button className="btn btn-outline-primary btn-sm"
                          onClick={() => openEdit(a)}>
                          <Pencil size={13} />
                        </button>
                        <button className="btn btn-outline-danger btn-sm"
                          onClick={() => excluirMut.mutate(a.id)}>
                          <Trash2 size={13} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {atividades.length === 0 && (
                  <tr>
                    <td colSpan={6} className="text-center text-muted py-4">
                      Nenhuma atividade encontrada.
                    </td>
                  </tr>
                )}
              </tbody>
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
                <h5 className="modal-title">
                  {editing ? 'Editar Atividade' : 'Nova Atividade'}
                </h5>
                <button className="btn-close" onClick={closeModal} />
              </div>
              <div className="modal-body">
                {editing ? (
                  // ── formulário de edição ──
                  <div className="row g-3">
                    <div className="col-12">
                      <label className="form-label fw-medium">Título *</label>
                      <input className="form-control" value={editForm.titulo}
                        onChange={e => setEditForm(f => ({ ...f, titulo: e.target.value }))} />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-medium">Tipo *</label>
                      <select className="form-select" value={editForm.tipo}
                        onChange={e => setEditForm(f => ({ ...f, tipo: Number(e.target.value) }))}>
                        {TIPOS.map(t => (
                          <option key={t.value} value={t.value}>{t.label}</option>
                        ))}
                      </select>
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-medium">Data Prevista *</label>
                      <input className="form-control" type="date" value={editForm.dataPrevista}
                        onChange={e => setEditForm(f => ({ ...f, dataPrevista: e.target.value }))} />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-medium">Status *</label>
                      <select className="form-select" value={editForm.status}
                        onChange={e => setEditForm(f => ({ ...f, status: Number(e.target.value) }))}>
                        {STATUS_ATIVIDADE.map(s => (
                          <option key={s.value} value={s.value}>{s.label}</option>
                        ))}
                      </select>
                    </div>
                    <div className="col-12">
                      <label className="form-label fw-medium">Observação</label>
                      <textarea className="form-control" rows={3} value={editForm.observacao}
                        onChange={e => setEditForm(f => ({ ...f, observacao: e.target.value }))} />
                    </div>
                  </div>
                ) : (
                  // ── formulário de criação ──
                  <div className="row g-3">
                    <div className="col-12">
                      <label className="form-label fw-medium">Título *</label>
                      <input className="form-control" value={criarForm.titulo}
                        onChange={e => setCriarForm(f => ({ ...f, titulo: e.target.value }))} />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-medium">Tipo *</label>
                      <select className="form-select" value={criarForm.tipo}
                        onChange={e => setCriarForm(f => ({ ...f, tipo: Number(e.target.value) }))}>
                        {TIPOS.map(t => (
                          <option key={t.value} value={t.value}>{t.label}</option>
                        ))}
                      </select>
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-medium">Data Prevista *</label>
                      <input className="form-control" type="date" value={criarForm.dataPrevista}
                        onChange={e => setCriarForm(f => ({ ...f, dataPrevista: e.target.value }))} />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label fw-medium">Lead ID (opcional)</label>
                      <input className="form-control" placeholder="GUID do Lead"
                        value={criarForm.leadId ?? ''}
                        onChange={e => setCriarForm(f => ({
                          ...f, leadId: e.target.value || undefined,
                        }))} />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label fw-medium">Oportunidade ID (opcional)</label>
                      <input className="form-control" placeholder="GUID da Oportunidade"
                        value={criarForm.oportunidadeId ?? ''}
                        onChange={e => setCriarForm(f => ({
                          ...f, oportunidadeId: e.target.value || undefined,
                        }))} />
                    </div>
                    <div className="col-md-6">
                      <label className="form-label fw-medium">Observação</label>
                      <textarea className="form-control" rows={3}
                        value={criarForm.observacao ?? ''}
                        onChange={e => setCriarForm(f => ({ ...f, observacao: e.target.value }))} />
                    </div>
                  </div>
                )}
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={closeModal}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!(editing ? editForm.titulo : criarForm.titulo) || isWorking}
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
