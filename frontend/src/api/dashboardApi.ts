import api from './axiosInstance'

export interface KanbanCardDto {
  id: string
  titulo: string
  contatoNome: string
  valor: number
}

export interface KanbanColunaDto {
  status: number
  statusNome: string
  total: number
  valorTotal: number
  cards: KanbanCardDto[]
}

export interface OportunidadesMesDto {
  mes: string
  total: number
  valor: number
}

export interface DashboardDto {
  totalContatos: number
  totalOportunidades: number
  valorPipeline: number
  taxaConversao: number
  ticketMedio: number
  kanbanColunas: KanbanColunaDto[]
  oportunidadesPorMes: OportunidadesMesDto[]
}

export const dashboardApi = {
  obter: () => api.get<DashboardDto>('/crm/dashboard').then(r => r.data),
}
