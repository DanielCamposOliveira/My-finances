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