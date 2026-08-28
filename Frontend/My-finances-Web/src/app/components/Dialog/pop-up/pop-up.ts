import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';

export interface DialogConfirmData {
  titulo: string;
  mensagem: string;
  textoConfirmar?: string;
  textoCancelar?: string;
}

@Component({
  selector: 'app-pop-up',
  standalone: true,
  imports: [
    MatButtonModule,
    MatDialogActions,
    MatDialogClose,
    MatDialogTitle,
    MatDialogContent
  ],
  templateUrl: './pop-up.html',
  styleUrl: './pop-up.scss',
})
export class PopUp {
  readonly dialogRef = inject(MatDialogRef<PopUp>);
  readonly data: DialogConfirmData = inject(MAT_DIALOG_DATA);
}