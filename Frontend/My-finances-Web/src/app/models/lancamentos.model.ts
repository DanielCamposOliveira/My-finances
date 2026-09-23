import { StatusParcelaEnum } from '../enums/status-parcela-enum';

// interface para atualizar o status da parcela
export interface LancamentoStatusParcelaModel
{
  parcelaId: number;
  status: StatusParcelaEnum
}

// interface para atualizar  o status da Conta Fixa
export interface LancamentoValorParcelaModel
{
  parcelaId: number;
  valorParcela: number;
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

export interface Lancamento
{
  id: number,
  descricao: string,
  valorBase: number,
  diaVencimento: number,
  ativo: boolean,
  categoriaId: number,
  tagIds?: number[]
}

export interface LancamentoResponseList
{
  id: number,
  Descricao: string,
  ValorTotal: number,
  ValorParcela: number,
  ParcelasRestante: string,
  DataVencimentoUltimaParcela: string,
  CategoriaNome: string,
  Tags?: string[]
}