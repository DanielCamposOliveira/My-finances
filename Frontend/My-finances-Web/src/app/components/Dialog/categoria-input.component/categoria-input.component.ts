import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { MatRadioChange } from '@angular/material/radio';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CategoriaCadastro } from '../../../models/categoria.model';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';
import { AtribuicaoEnum } from '../../../enums/atribuicao-enum';

/** Interface que define os textos e labels dinâmicos da modal */
export interface DialogDataCategoria {
  titulo: string;
  label?: string;
  placeholder: string;
  textoConfirmar?: string;
  textoCancelar?: string;
}

@Component({
  selector: 'app-categoria-input.component',
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatRadioModule,
    MatDialogActions,
    MatDialogClose,
    MatDialogTitle,
    MatDialogContent,
  ],
  templateUrl: './categoria-input.component.html',
  styleUrl: './categoria-input.component.scss',
})
export class CategoriaInputComponent {
  // Injeção de dependência para controle e fechamento da modal
  readonly dialogRef = inject(MatDialogRef<CategoriaInputComponent>);

  // Dados recebidos de quem abriu a modal
  readonly data: DialogDataCategoria = inject(MAT_DIALOG_DATA);

  // Expondo o Enum para o HTML
  public readonly AtribuicaoEnum = AtribuicaoEnum;

  public tagPayload: CategoriaCadastro = {
    nome: '',
    atribuicao: AtribuicaoEnum.Despesa
  };

  /**
 * Atualiza o nome da tag conforme o usuário digita no input
 * @param event Evento padrão de input HTML
 */

  onNomeInput(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.tagPayload.nome = inputElement.value;
    
  }

  // Dentro da classe CategoriaInputComponent:
onAtribuicaoChange(event: MatRadioChange): void {
  this.tagPayload.atribuicao = event.value;
}


}
