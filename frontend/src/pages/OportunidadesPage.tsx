import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { PlusCircle } from 'lucide-react'
import { oportunidadeApi, CriarOportunidadeRequest, StatusOportunidade } from '../api/oportunidadeApi'
import { contatoApi } from '../api/contatoApi'

const STATUS_BADGE: Record<number, string> = {
  1: 'badge-aberta',
  2: 'badge-andamento',
  3: 'badge-ganha',
  4: 'badge-perdida',
  5: 'badge-cancelada',
}

export default function OportunidadesPage() {
  const qc = useQueryClient()
  const [showModal, setShowModal] = useState(false)
  const [form, setForm] = useState<CriarOportunidadeRequest>({
    contatoId: '', titulo: '', valor: 0,
  })

  const { data: oportunidades = [], isLoading } = useQuery({
    queryKey: ['oportunidades'],
    queryFn: oportunidadeApi.listar,
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
      setForm({ contatoId: '', titulo: '', valor: 0 })
    },
  })

  const avancarMutation = useMutation({
    mutationFn: ({ id, novoStatus }: { id: string; novoStatus: number }) =>
      oportunidadeApi.avancarStatus(id, novoStatus),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['oportunidades'] }),
  })

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
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>Título</th>
                  <th>Contato</th>
                  <th>Valor</th>
                  <th>Previsão</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {oportunidades.map(o => (
                  <tr key={o.id}>
                    <td className="fw-medium">{o.titulo}</td>
                    <td className="small text-muted">{o.contatoNome}</td>
                    <td>R$ {o.valor.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}</td>
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
                            onClick={() => avancarMutation.mutate({ id: o.id, novoStatus: StatusOportunidade.EmAndamento })}>
                            Iniciar
                          </button>
                        )}
                        {(o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento) && (
                          <>
                            <button className="btn btn-outline-success btn-sm"
                              onClick={() => avancarMutation.mutate({ id: o.id, novoStatus: StatusOportunidade.Ganha })}>
                              Ganhar
                            </button>
                            <button className="btn btn-outline-danger btn-sm"
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
                  <tr><td colSpan={6} className="text-center text-muted py-4">Nenhuma oportunidade cadastrada.</td></tr>
                )}
              </tbody>
              {oportunidades.length > 0 && (() => {
                const totalPipeline = oportunidades
                  .filter(o => o.status === StatusOportunidade.Aberta || o.status === StatusOportunidade.EmAndamento)
                  .reduce((s, o) => s + o.valor, 0)
                return (
                  <tfoot className="table-light">
                    <tr>
                      <td colSpan={2} className="fw-semibold">Pipeline Ativo</td>
                      <td className="fw-bold">
                        R$ {totalPipeline.toLocaleString('pt-BR', { minimumFractionDigits: 0 })}
                      </td>
                      <td colSpan={3} />
                    </tr>
                  </tfoot>
                )
              })()}
            </table>
          </div>
        )}
      </div>

      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Nova Oportunidade</h5>
                <button className="btn-close" onClick={() => setShowModal(false)} />
              </div>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label fw-medium">Contato *</label>
                  <select className="form-select" value={form.contatoId}
                    onChange={e => setForm(f => ({ ...f, contatoId: e.target.value }))}>
                    <option value="">Selecionar...</option>
                    {contatos.filter(c => c.status === 1).map(c => (
                      <option key={c.id} value={c.id}>{c.nome} {c.empresa ? `— ${c.empresa}` : ''}</option>
                    ))}
                  </select>
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Título *</label>
                  <input className="form-control" value={form.titulo}
                    onChange={e => setForm(f => ({ ...f, titulo: e.target.value }))} />
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Valor (R$) *</label>
                  <input className="form-control" type="number" min="0" value={form.valor}
                    onChange={e => setForm(f => ({ ...f, valor: Number(e.target.value) }))} />
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
                <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!form.contatoId || !form.titulo || criarMutation.isPending}
                  onClick={() => criarMutation.mutate(form)}>
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
