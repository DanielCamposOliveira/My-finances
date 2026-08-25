import { StatusParcelaEnum } from '../enums/status-parcela-enum';

// interface de cadastro Conta Fixa
export interface ContaFixaCadastroModel
{
  descricao: string;
  valorBase: number;
  diaVencimento: number;
  categoriaId: number;
  tagIds?: number[];
}

// interface para atualizar  o status da Conta Fixa
export interface ContaFixaStatusModel
{
  id_ContaFixa: number;
  status: boolean;
}

// interface para atualizar  o status da Conta Fixa
export interface ContaFixaStatusParcelaModel
{
  parcelaId: number;
  status: StatusParcelaEnum;
}

// interface para atualizar  o status da Conta Fixa
export interface ContaFixaValorParcelaModel
{
  parcelaId: number;
  valorParcela: number;
}


// interface para as parcelas dos lancamentos
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