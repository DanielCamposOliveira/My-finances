import { StatusParcelaEnum } from '../enums/status-parcela-enum';

export interface returnParcela {
  id: number;
  dependence_id: number;
  descricao: string;
  valorParcela: number;
  dataVencimento: string;
  status: StatusParcelaEnum;
  atribuicao: number;
  numeroParcela: number;
}