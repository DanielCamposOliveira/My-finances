export enum StatusParcela {
  Aberto = 1,
  Pago = 2,
  Atrasado = 3,
  Cancelado = 4,
}

export enum Atribuicao {
  Despesa = 1,
  Ganho = 2,
}

export interface ContaPendenteItem {
  id: number;
  origemId: number; // contaFixaId ou lancamento_Id
  descricao: string; // descricao ou lancamento_Descricao
  valorParcela: number;
  dataVencimento: string;
  status: StatusParcela | number; // Recebe o número da API (1, 2, 3, 4)
  tipo: 'CONTA_FIXA' | 'LANCAMENTO';
  numeroParcela?: number;
  dataPagamento?: string | null;
}
