import api from './axiosInstance'

export interface ClienteCoreDto {
  id: number
  nome: string
  email: string
  telefone?: string
  cnpj?: string
  cpf?: string
}

export interface ImportacaoResult {
  importados: number
  ignorados: number
}

export const integracaoApi = {
  getClientesCore: () =>
    api.get<ClienteCoreDto[]>('/integracao/core/clientes').then(r => r.data),

  importarDeCore: (clienteIds: number[]) =>
    api.post<ImportacaoResult>('/lead/importar-de-core', { clienteIds }).then(r => r.data),
}
