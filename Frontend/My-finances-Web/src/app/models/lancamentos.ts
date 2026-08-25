import { StatusParcelaEnum } from '../enums/status-parcela-enum';

// interface para cadastro
export interface LancamentosModel
{
  descricao: string;
  valorTotal: number;
  categoriaId: number;
  dataPrimeiroVencimento: string;
  //tagIds: Tag[];
  tagIds?: number  [];
}

// interface para atualizar o status da parcela
export interface LancamentoParcelaModel
{
  parcelaId: number;
  status: StatusParcelaEnum
}






export interface returnParcelaModel {
  id: number;
  dependence_id: number;
  descricao: string;
  valorParcela: number;
  dataVencimento: number;
  status: StatusParcelaEnum;
  atribuicao: number;
  numeroParcela: number[];
}