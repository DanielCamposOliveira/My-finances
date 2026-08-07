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

export interface CategoriaModel
{
  id: number;
  nome: string;
  atribuicao: AtribuicaoModel;
}


export interface TagModel
{
  id: number;
  nome: string;
}

export interface TagCadastorModel
{
  nome: string;
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
  status: StatusParcelaModel
}



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
  status: StatusParcelaModel;
}

// interface para atualizar  o status da Conta Fixa
export interface ContaFixaValorParcelaModel
{
  parcelaId: number;
  valorParcela: number;
}