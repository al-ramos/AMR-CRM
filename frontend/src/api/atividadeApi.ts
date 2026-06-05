import api from './axiosInstance'

export interface AtividadeDto {
  id: string
  titulo: string
  tipo: number
  tipoNome: string
  dataPrevista: string
  status: number
  statusNome: string
  observacao?: string
  leadId?: string
  leadNome?: string
  oportunidadeId?: string
  oportunidadeTitulo?: string
  vencida: boolean
  criadoEm: string
  alteradoEm: string
}

export interface CriarAtividadeRequest {
  titulo: string
  tipo: number
  dataPrevista: string
  leadId?: string
  oportunidadeId?: string
  observacao?: string
}

export interface AtualizarAtividadeRequest {
  titulo: string
  tipo: number
  dataPrevista: string
  status: number
  observacao?: string
}

export const TipoAtividade = {
  Ligacao: 1,
  Email:   2,
  Reuniao: 3,
  Tarefa:  4,
  Visita:  5,
} as const

export const StatusAtividade = {
  Pendente:  1,
  Concluida: 2,
  Cancelada: 3,
} as const

export const TIPOS = [
  { value: 1, label: 'Ligação' },
  { value: 2, label: 'E-mail' },
  { value: 3, label: 'Reunião' },
  { value: 4, label: 'Tarefa' },
  { value: 5, label: 'Visita' },
]

export const STATUS_ATIVIDADE = [
  { value: 1, label: 'Pendente' },
  { value: 2, label: 'Concluída' },
  { value: 3, label: 'Cancelada' },
]

export const atividadeApi = {
  listar:   (status?: number, tipo?: number) =>
    api.get<AtividadeDto[]>('/atividades', {
      params: { ...(status !== undefined && { status }), ...(tipo !== undefined && { tipo }) },
    }).then(r => r.data),
  vencidas: ()                                      => api.get<AtividadeDto[]>('/atividades/vencidas').then(r => r.data),
  obter:    (id: string)                            => api.get<AtividadeDto>(`/atividades/${id}`).then(r => r.data),
  criar:    (req: CriarAtividadeRequest)            => api.post<AtividadeDto>('/atividades', req).then(r => r.data),
  atualizar:(id: string, req: AtualizarAtividadeRequest) => api.put(`/atividades/${id}`, req),
  excluir:  (id: string)                            => api.delete(`/atividades/${id}`),
}
