import api from './axiosInstance'

export interface ContatoDto {
  id: string
  nome: string
  email: string
  telefone?: string
  empresa?: string
  tipo: number
  tipoNome: string
  status: number
  statusNome: string
  notas?: string
  criadoEm: string
}

export interface CriarContatoRequest {
  nome: string
  email: string
  tipo: number
  telefone?: string
  empresa?: string
  notas?: string
}

export const contatoApi = {
  listar: () => api.get<ContatoDto[]>('/contato').then(r => r.data),
  obter:  (id: string) => api.get<ContatoDto>(`/contato/${id}`).then(r => r.data),
  criar:  (req: CriarContatoRequest) => api.post<ContatoDto>('/contato', req).then(r => r.data),
  atualizar: (id: string, req: CriarContatoRequest) =>
    api.put(`/contato/${id}`, req),
  inativar: (id: string) => api.patch(`/contato/${id}/inativar`),
}
