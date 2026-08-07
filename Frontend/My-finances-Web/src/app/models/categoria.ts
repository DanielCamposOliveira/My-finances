import { AtribuicaoEnum } from '../enums/atribuicao-enum';


export interface CategoriaModel
{
  id: number;
  nome: string;
  atribuicao: AtribuicaoEnum;
}

export interface CategoriaCadastroModel
{
  nome: string;
  atribuicao: AtribuicaoEnum;
}