import api from './axiosInstance'

export interface LeadDto {
  id: string
  nome: string
  email: string
  telefone?: string
  empresa?: string
  status: number
  statusNome: string
  origem: number
  origemNome: string
  valorEstimado: number
  notas?: string
  criadoEm: string
}

export interface CriarLeadRequest {
  nome: string
  email: string
  origem: number
  valorEstimado: number
  telefone?: string
  empresa?: string
  notas?: string
}

export const StatusLead = {
  Novo: 1, Qualificado: 2, Proposta: 3, Ganho: 4, Perdido: 5,
} as const

export const OrigemLead = {
  Site: 1, Indicacao: 2, LinkedIn: 3, Email: 4, Evento: 5, Outro: 6,
} as const

export const ORIGEM_LABEL: Record<number, string> = {
  1: 'Site', 2: 'Indicação', 3: 'LinkedIn', 4: 'E-mail', 5: 'Evento', 6: 'Outro',
}

export const leadApi = {
  listar:     ()                              => api.get<LeadDto[]>('/lead').then(r => r.data),
  obter:      (id: string)                   => api.get<LeadDto>(`/lead/${id}`).then(r => r.data),
  criar:      (req: CriarLeadRequest)        => api.post<LeadDto>('/lead', req).then(r => r.data),
  atualizar:  (id: string, req: CriarLeadRequest) => api.put(`/lead/${id}`, req),
  excluir:    (id: string)                   => api.delete(`/lead/${id}`),
  qualificar: (id: string)                   => api.patch(`/lead/${id}/qualificar`),
  proposta:   (id: string)                   => api.patch(`/lead/${id}/proposta`),
  ganhar:     (id: string)                   => api.patch(`/lead/${id}/ganhar`),
  perder:     (id: string)                   => api.patch(`/lead/${id}/perder`),
}
