import api from './axiosInstance'

export interface OportunidadeDto {
  id: string
  contatoId?: string
  contatoNome?: string
  leadId?: string
  leadNome?: string
  titulo: string
  valor: number
  probabilidade: number
  status: number
  statusNome: string
  descricao?: string
  previsaoFechamento?: string
  criadoEm: string
}

export interface CriarOportunidadeRequest {
  titulo: string
  valor: number
  contatoId?: string
  leadId?: string
  probabilidade?: number
  descricao?: string
  previsaoFechamento?: string
}

export interface AtualizarOportunidadeRequest {
  titulo: string
  valor: number
  probabilidade?: number
  descricao?: string
  previsaoFechamento?: string
}

export const StatusOportunidade = {
  Aberta: 1,
  EmAndamento: 2,
  Ganha: 3,
  Perdida: 4,
  Cancelada: 5,
} as const

export const oportunidadeApi = {
  listar:    () => api.get<OportunidadeDto[]>('/oportunidade').then(r => r.data),
  obter:     (id: string) => api.get<OportunidadeDto>(`/oportunidade/${id}`).then(r => r.data),
  criar:     (req: CriarOportunidadeRequest) =>
    api.post<OportunidadeDto>('/oportunidade', req).then(r => r.data),
  atualizar: (id: string, req: AtualizarOportunidadeRequest) =>
    api.put<OportunidadeDto>(`/oportunidade/${id}`, req).then(r => r.data),
  excluir:   (id: string) => api.delete(`/oportunidade/${id}`),
  iniciar:   (id: string) => api.patch(`/oportunidade/${id}/iniciar`),
  ganhar:    (id: string) => api.patch(`/oportunidade/${id}/ganhar`),
  perder:    (id: string) => api.patch(`/oportunidade/${id}/perder`),
  cancelar:  (id: string) => api.patch(`/oportunidade/${id}/cancelar`),
}
