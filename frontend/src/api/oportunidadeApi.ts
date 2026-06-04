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
  probabilidade?: number
  contatoId?: string
  leadId?: string
  descricao?: string
  previsaoFechamento?: string
}

export const StatusOportunidade = {
  Aberta:      1,
  EmAndamento: 2,
  Ganha:       3,
  Perdida:     4,
  Cancelada:   5,
} as const

export const oportunidadeApi = {
  listar: () => api.get<OportunidadeDto[]>('/oportunidade').then(r => r.data),
  criar:  (req: CriarOportunidadeRequest) =>
    api.post<OportunidadeDto>('/oportunidade', req).then(r => r.data),
  avancarStatus: (id: string, novoStatus: number) =>
    api.patch(`/oportunidade/${id}/status`, { novoStatus }),
}
