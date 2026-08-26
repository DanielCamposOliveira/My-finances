import { StatusParcelaEnum } from '../enums/status-parcela-enum';

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