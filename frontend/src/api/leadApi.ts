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
  alteradoEm: string
  origemCoreClienteId?: number
}

export interface CriarLeadRequest {
  nome: string
  email: string
  origem: number
  telefone?: string
  empresa?: string
  valorEstimado: number
  notas?: string
}

export const StatusLead = {
  Novo:        1,
  Qualificado: 2,
  Proposta:    3,
  Ganho:       4,
  Perdido:     5,
} as const

export const OrigemLead = {
  Manual:    1,
  Website:   2,
  LinkedIn:  3,
  Indicacao: 4,
  Email:     5,
  Evento:    6,
  Outro:     7,
} as const

export const ORIGENS = [
  { value: 1, label: 'Manual' },
  { value: 2, label: 'Website' },
  { value: 3, label: 'LinkedIn' },
  { value: 4, label: 'Indicação' },
  { value: 5, label: 'E-mail' },
  { value: 6, label: 'Evento' },
  { value: 7, label: 'Outro' },
]

export const leadApi = {
  listar:  (status?: number) =>
    api.get<LeadDto[]>('/lead', { params: status !== undefined ? { status } : {} }).then(r => r.data),
  obter:   (id: string)                      => api.get<LeadDto>(`/lead/${id}`).then(r => r.data),
  criar:   (req: CriarLeadRequest)           => api.post<LeadDto>('/lead', req).then(r => r.data),
  atualizar: (id: string, req: CriarLeadRequest) => api.put(`/lead/${id}`, req),
  avancarStatus: (id: string, novoStatus: number) =>
    api.patch(`/lead/${id}/status`, { novoStatus }),
  excluir: (id: string)                      => api.delete(`/lead/${id}`),
}
