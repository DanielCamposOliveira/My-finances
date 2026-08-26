import { StatusParcelaEnum } from '../enums/status-parcela-enum';
import { AtribuicaoEnum } from '../enums/atribuicao-enum';

export interface ContaPendenteItemModel {
  id: number;
  origemId: number; // contaFixaId ou lancamento_Id
  descricao: string; // descricao ou lancamento_Descricao
  valorParcela: number;
  dataVencimento: string;
  status: StatusParcelaEnum | number; // Recebe o número da API (1, 2, 3, 4)
  //tipo: 'CONTA_FIXA' | 'LANCAMENTO';
  numeroParcela?: number;
  dataPagamento?: string | null;
  atribuicao : AtribuicaoEnum | number; // Recebe o número da API (1, 2)
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
