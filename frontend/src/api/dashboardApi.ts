import api from './axiosInstance'

export interface DashboardKpisDto {
  totalLeads: number
  leadsAtivos: number
  oportunidadesAbertas: number
  pipelinePonderado: number
  taxaConversao: number
}

export interface FunilLeadItemDto {
  status: string
  count: number
  valor: number
}

export interface FunilEtapaItemDto {
  etapa: string
  count: number
  valor: number
}

export interface FunilDto {
  leads: FunilLeadItemDto[]
  oportunidades: FunilEtapaItemDto[]
}

export interface TopOportunidadeDto {
  id: string
  titulo: string
  lead: string | null
  contato: string | null
  valor: number
  probabilidade: number
  valorPonderado: number
  etapa: string
  status: string
}

export interface EvolucaoItemDto {
  mes: string
  abertas: number
  fechadas: number
}

export const dashboardApi = {
  kpis: () =>
    api.get<DashboardKpisDto>('/dashboard').then(r => r.data),
  funil: () =>
    api.get<FunilDto>('/dashboard/funil').then(r => r.data),
  topOportunidades: () =>
    api.get<TopOportunidadeDto[]>('/dashboard/top-oportunidades').then(r => r.data),
  evolucao: (periodo: number) =>
    api.get<EvolucaoItemDto[]>(`/dashboard/evolucao?periodo=${periodo}`).then(r => r.data),
}
