import api from './axiosInstance'

export interface FunilConversaoItemDto {
  etapa: string
  total: number
  valor: number
  taxaConversao: number
}

export interface FunilConversaoDto {
  itens: FunilConversaoItemDto[]
  taxaConversaoGeral: number
  periodoDias: number
}

export interface ForecastOportunidadeDto {
  id: string
  titulo: string
  valor: number
  probabilidade: number
  valorPonderado: number
  previsaoFechamento: string
  status: string
  etapa: string
}

export interface ForecastDto {
  mes: string
  receitaEsperada: number
  receitaConfirmada: number
  totalOportunidades: number
  oportunidades: ForecastOportunidadeDto[]
}

export interface LeadsPorOrigemItemDto {
  origem: string
  total: number
  valorTotal: number
}

export interface LeadsPorStatusItemDto {
  status: string
  total: number
  valorTotal: number
}

export interface AnaliseLeadsDto {
  porOrigem: LeadsPorOrigemItemDto[]
  porStatus: LeadsPorStatusItemDto[]
  totalLeads: number
  valorTotalPipeline: number
  periodoDias: number
}

export const relatorioApi = {
  funil: (periodo: number) =>
    api.get<FunilConversaoDto>(`/relatorios/funil?periodo=${periodo}`).then(r => r.data),

  forecast: (mes: string) =>
    api.get<ForecastDto>(`/relatorios/forecast?mes=${mes}`).then(r => r.data),

  leads: (periodo: number) =>
    api.get<AnaliseLeadsDto>(`/relatorios/leads?periodo=${periodo}`).then(r => r.data),

  exportFunilUrl:    (periodo: number) => `/api/relatorios/funil/export?periodo=${periodo}`,
  exportForecastUrl: (mes: string)     => `/api/relatorios/forecast/export?mes=${mes}`,
  exportLeadsUrl:    (periodo: number) => `/api/relatorios/leads/export?periodo=${periodo}`,
}
