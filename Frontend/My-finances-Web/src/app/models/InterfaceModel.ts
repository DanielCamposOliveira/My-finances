export enum StatusParcelaModel {
  Aberto = 1,
  Pago = 2,
  Atrasado = 3,
  Cancelado = 4,
}

export enum AtribuicaoModel {
  Despesa = 1,
  Ganho = 2,
}

export interface ContaPendenteItemModel {
  id: number;
  origemId: number; // contaFixaId ou lancamento_Id
  descricao: string; // descricao ou lancamento_Descricao
  valorParcela: number;
  dataVencimento: string;
  status: StatusParcelaModel | number; // Recebe o número da API (1, 2, 3, 4)
  tipo: 'CONTA_FIXA' | 'LANCAMENTO';
  numeroParcela?: number;
  dataPagamento?: string | null;
  atribuicao : AtribuicaoModel | number; // Recebe o número da API (1, 2)
}

// Interfaces para os dados do histórico financeiro
export interface ChartSeriesModel {
  type?: string;
  name: string;
  color: string;
  data: number[];
}

// Interface para o histórico financeiro anual
export interface HistoricoFinanceiroAnualModel {
  chartCategories: string[];
   chartSeries: ChartSeriesModel[];
}

export interface DashboardCardItemModel {
  title: string;
  value: number;
  iconClass: string;
  typeClass: string;
}