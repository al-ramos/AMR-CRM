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
  etapa: number
  etapaNome: string
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
  etapa?: number
  contatoId?: string
  leadId?: string
  descricao?: string
  previsaoFechamento?: string
}

export interface AtualizarOportunidadeRequest {
  titulo: string
  valor: number
  probabilidade?: number
  etapa?: number
  descricao?: string
  previsaoFechamento?: string
}

export const EtapaOportunidade = {
  Prospeccao:   1,
  Qualificacao: 2,
  Proposta:     3,
  Negociacao:   4,
  Fechamento:   5,
} as const

export const ETAPA_LABEL: Record<number, string> = {
  1: 'Prospecção',
  2: 'Qualificação',
  3: 'Proposta',
  4: 'Negociação',
  5: 'Fechamento',
}

export const ETAPA_COLOR: Record<number, string> = {
  1: 'bg-secondary',
  2: 'bg-info',
  3: 'bg-warning text-dark',
  4: 'bg-primary',
  5: 'bg-success',
}

export const StatusOportunidade = {
  Aberta:      1,
  EmAndamento: 2,
  Ganha:       3,
  Perdida:     4,
  Cancelada:   5,
} as const

export const oportunidadeApi = {
  listar: () =>
    api.get<OportunidadeDto[]>('/oportunidade').then(r => r.data),
  obterPorId: (id: string) =>
    api.get<OportunidadeDto>(`/oportunidade/${id}`).then(r => r.data),
  criar: (req: CriarOportunidadeRequest) =>
    api.post<OportunidadeDto>('/oportunidade', req).then(r => r.data),
  atualizar: (id: string, req: AtualizarOportunidadeRequest) =>
    api.put(`/oportunidade/${id}`, req),
  excluir: (id: string) =>
    api.delete(`/oportunidade/${id}`),
  avancarStatus: (id: string, novoStatus: number) =>
    api.patch(`/oportunidade/${id}/status`, { novoStatus }),
}
