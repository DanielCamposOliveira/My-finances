import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TagCadastro } from '../../../models/tag.model';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';

/** Interface que define os textos e labels dinâmicos da modal */
export interface DialogDataTag {
  titulo: string;
  label?: string;
  placeholder: string;
  textoConfirmar?: string;
  textoCancelar?: string;
}


@Component({
  selector: 'app-tag-input',
  standalone: true,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogActions,
    MatDialogClose,
    MatDialogTitle,
    MatDialogContent,
  ],
  templateUrl: './tag-input.component.html',
  styleUrl: './tag-input.component.scss',
})
export class TagInputComponent {
  // Injeção de dependência para controle e fechamento da modal
  readonly dialogRef = inject(MatDialogRef<TagInputComponent>);
  
  // Dados recebidos de quem abriu a modal
  readonly data: DialogDataTag = inject(MAT_DIALOG_DATA);

  public tagPayload: TagCadastro = {
    nome: ''
  };

  /**
   * Atualiza o nome da tag conforme o usuário digita no input
   * @param event Evento padrão de input HTML
   */

  onTagInput(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
     this.tagPayload.nome = inputElement.value;
  }

  
}
