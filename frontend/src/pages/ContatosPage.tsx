import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { UserPlus, Search } from 'lucide-react'
import { contatoApi, CriarContatoRequest } from '../api/contatoApi'

const TIPOS = [
  { value: 1, label: 'Lead' },
  { value: 2, label: 'Prospect' },
  { value: 3, label: 'Cliente' },
  { value: 4, label: 'Parceiro' },
]

const tipoBadge: Record<number, string> = {
  1: 'bg-info text-dark',
  2: 'bg-warning text-dark',
  3: 'bg-success',
  4: 'bg-secondary',
}

export default function ContatosPage() {
  const qc = useQueryClient()
  const [busca, setBusca] = useState('')
  const [showModal, setShowModal] = useState(false)
  const [form, setForm] = useState<CriarContatoRequest>({
    nome: '', email: '', tipo: 1,
  })

  const { data: contatos = [], isLoading } = useQuery({
    queryKey: ['contatos'],
    queryFn: contatoApi.listar,
  })

  const criarMutation = useMutation({
    mutationFn: contatoApi.criar,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['contatos'] })
      setShowModal(false)
      setForm({ nome: '', email: '', tipo: 1 })
    },
  })

  const inativarMutation = useMutation({
    mutationFn: contatoApi.inativar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['contatos'] }),
  })

  const filtrados = contatos.filter(c =>
    c.nome.toLowerCase().includes(busca.toLowerCase()) ||
    c.email.toLowerCase().includes(busca.toLowerCase()) ||
    (c.empresa ?? '').toLowerCase().includes(busca.toLowerCase()),
  )

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h4 className="fw-bold mb-0">Contatos</h4>
        <button className="btn btn-primary btn-sm d-flex align-items-center gap-2"
          onClick={() => setShowModal(true)}>
          <UserPlus size={15} /> Novo Contato
        </button>
      </div>

      <div className="card card-kpi p-3 mb-3">
        <div className="input-group">
          <span className="input-group-text bg-white border-end-0">
            <Search size={15} className="text-muted" />
          </span>
          <input
            className="form-control border-start-0"
            placeholder="Buscar por nome, e-mail ou empresa..."
            value={busca}
            onChange={e => setBusca(e.target.value)}
          />
        </div>
      </div>

      <div className="card card-kpi">
        {isLoading ? (
          <div className="p-4 text-center text-muted">Carregando...</div>
        ) : (
          <div className="table-responsive">
            <table className="table table-hover mb-0">
              <thead className="table-light">
                <tr>
                  <th>Nome</th>
                  <th>E-mail</th>
                  <th>Empresa</th>
                  <th>Tipo</th>
                  <th>Status</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {filtrados.map(c => (
                  <tr key={c.id}>
                    <td className="fw-medium">{c.nome}</td>
                    <td className="text-muted small">{c.email}</td>
                    <td className="small">{c.empresa ?? '—'}</td>
                    <td>
                      <span className={`badge ${tipoBadge[c.tipo] ?? 'bg-secondary'}`}>
                        {c.tipoNome}
                      </span>
                    </td>
                    <td>
                      <span className={`badge ${c.status === 1 ? 'bg-success' : 'bg-secondary'}`}>
                        {c.statusNome}
                      </span>
                    </td>
                    <td>
                      {c.status === 1 && (
                        <button
                          className="btn btn-outline-danger btn-sm"
                          onClick={() => inativarMutation.mutate(c.id)}>
                          Inativar
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
                {filtrados.length === 0 && (
                  <tr><td colSpan={6} className="text-center text-muted py-4">Nenhum contato encontrado.</td></tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {showModal && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Novo Contato</h5>
                <button className="btn-close" onClick={() => setShowModal(false)} />
              </div>
              <div className="modal-body">
                <div className="mb-3">
                  <label className="form-label fw-medium">Nome *</label>
                  <input className="form-control" value={form.nome}
                    onChange={e => setForm(f => ({ ...f, nome: e.target.value }))} />
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">E-mail *</label>
                  <input className="form-control" type="email" value={form.email}
                    onChange={e => setForm(f => ({ ...f, email: e.target.value }))} />
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Tipo *</label>
                  <select className="form-select" value={form.tipo}
                    onChange={e => setForm(f => ({ ...f, tipo: Number(e.target.value) }))}>
                    {TIPOS.map(t => <option key={t.value} value={t.value}>{t.label}</option>)}
                  </select>
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Telefone</label>
                  <input className="form-control" value={form.telefone ?? ''}
                    onChange={e => setForm(f => ({ ...f, telefone: e.target.value }))} />
                </div>
                <div className="mb-3">
                  <label className="form-label fw-medium">Empresa</label>
                  <input className="form-control" value={form.empresa ?? ''}
                    onChange={e => setForm(f => ({ ...f, empresa: e.target.value }))} />
                </div>
              </div>
              <div className="modal-footer">
                <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Cancelar</button>
                <button className="btn btn-primary"
                  disabled={!form.nome || !form.email || criarMutation.isPending}
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
