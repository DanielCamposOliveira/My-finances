import { AtribuicaoEnum } from '../enums/atribuicao-enum';


export interface Categoria
{
  id: number;
  nome: string;
  atribuicao: AtribuicaoEnum;
}

export interface CategoriaCadastro
{
  nome: string;
  atribuicao: AtribuicaoEnum;
}