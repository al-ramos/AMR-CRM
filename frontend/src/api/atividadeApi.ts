import api from './axiosInstance'

export interface AtividadeDto {
  id: string
  oportunidadeId: string
  tipo: number
  tipoNome: string
  descricao: string
  dataHora: string
  concluida: boolean
  criadoEm: string
}

export interface CriarAtividadeRequest {
  oportunidadeId: string
  tipo: number
  descricao: string
  dataHora: string
}

export interface AtualizarAtividadeRequest {
  tipo: number
  descricao: string
  dataHora: string
}

export const TipoAtividade = {
  Ligacao: 1,
  Reuniao: 2,
  Email:   3,
  Tarefa:  4,
} as const

export const TIPO_LABEL: Record<number, string> = {
  1: 'Ligação',
  2: 'Reunião',
  3: 'E-mail',
  4: 'Tarefa',
}

export const atividadeApi = {
  listar:   (oportunidadeId: string) =>
    api.get<AtividadeDto[]>(`/atividade/oportunidade/${oportunidadeId}`).then(r => r.data),
  criar:    (req: CriarAtividadeRequest) =>
    api.post<AtividadeDto>('/atividade', req).then(r => r.data),
  atualizar: (id: string, req: AtualizarAtividadeRequest) =>
    api.put<AtividadeDto>(`/atividade/${id}`, req).then(r => r.data),
  excluir:  (id: string) => api.delete(`/atividade/${id}`),
  concluir: (id: string) => api.patch(`/atividade/${id}/concluir`),
}
