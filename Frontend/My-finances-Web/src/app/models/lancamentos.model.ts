import { StatusParcelaEnum } from '../enums/status-parcela-enum';

// interface para atualizar o status da parcela
export interface LancamentoStatusParcelaModel
{
  parcelaId: number;
  status: StatusParcelaEnum
}

export interface LancamentoCadastro
{
  descricao: string,
  valorTotal: number,
  qtdParcelas: number,
  dataPrimeiroVencimento: string,
  categoriaId: number,
  tagIds?: number[]
}