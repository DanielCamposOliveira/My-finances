import { Component, inject, model, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle,
} from '@angular/material/dialog';

import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';

registerLocaleData(localePt);


export interface DialogEditarValorData {
  titulo: string;
  label?: string;
  valorInicial: number;
  textoConfirmar?: string;
  textoCancelar?: string;
}

@Component({
  selector: 'app-dialog-input',
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
  templateUrl: './dialog-input.component.html',
  styleUrl: './dialog-input.component.scss',
})
export class DialogInputComponent implements OnInit {
  readonly dialogRef = inject(MatDialogRef<DialogInputComponent>);
  
  readonly data: DialogEditarValorData = inject(MAT_DIALOG_DATA);

  // Valor numérico real
  valorNumerico = 0;

  // Texto formatado exibido no input (ex: "R$ 1.000,00")
  valorFormatado = '';

  ngOnInit(): void {
    const inicial = Number(this.data.valorInicial) || 0;
    this.valorNumerico = inicial;
    this.valorFormatado = this.formatarMoedaBRL(inicial);
  }

  // Formata o número para o padrão pt-BR de moeda
  private formatarMoedaBRL(valor: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  }

  // Trata a digitação no input em tempo real estilo máscara de centavos
  onInputMoeda(event: Event): void {
    const input = event.target as HTMLInputElement;
    const apenasDigitos = input.value.replace(/\D/g, '');

    if (!apenasDigitos) {
      this.valorNumerico = 0;
      this.valorFormatado = this.formatarMoedaBRL(0);
      return;
    }

    // Trata os dígitos como centavos
    this.valorNumerico = Number(apenasDigitos) / 100;
    this.valorFormatado = this.formatarMoedaBRL(this.valorNumerico);
  }
}