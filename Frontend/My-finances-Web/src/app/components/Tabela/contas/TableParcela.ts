import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';

import { PopUp } from '../../Dialog/pop-up/pop-up'

import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarCustomComponent } from '../../SnackBar/snack-bar-info/snack-bar-info';



import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';

import { AtribuicaoEnum } from '../../../enums/atribuicao-enum';
import { StatusParcelaEnum } from '../../../enums/status-parcela-enum';
import { returnParcela } from "../../../models/parcela-model";

import {
  MatDialog,
} from '@angular/material/dialog';


registerLocaleData(localePt);

export type TipoSnack = 'sucesso' | 'erro' | 'info' | 'despesa' | 'receita';

@Component({
  selector: 'app-TableContasFixa',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './TableParcela.html',
  styleUrl: './TableParcela.scss',
})
  


export class TableParcela implements OnInit, OnChanges {
  private cdr = inject(ChangeDetectorRef);

  @Input() titulo: string = 'Parcelas';
  @Input() parcelas: returnParcela[] = [];

  @Output() onPagar = new EventEmitter<returnParcela>();

  readonly StatusParcelaEnum = StatusParcelaEnum;

  readonly dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  
  displayedColumns: string[] = [
    'id',
    'numeroParcela',
    'descricao',
    'valorParcela',
    'dataVencimento',
    'status',
    'atribuicao',
    'acoes'
  ];

  dataSource = new MatTableDataSource<returnParcela>([]);

  statusOptions = Object.keys(StatusParcelaEnum)
    .filter(key => isNaN(Number(key)))
    .map(key => ({
      label: key,
      value: StatusParcelaEnum[key as keyof typeof StatusParcelaEnum] as number
    }));

  filtroTexto: string = '';
  filtroStatus: number | null = null;

  ngOnInit(): void {
    this.atualizarDataSource(this.parcelas || []);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['parcelas'] && this.parcelas) {
      this.atualizarDataSource(this.parcelas);
    }
  }

  private atualizarDataSource(dados: returnParcela[]): void {
    this.dataSource = new MatTableDataSource<returnParcela>(dados);
    this.configurarFiltro();
    this.atualizarFiltro();
    this.cdr.markForCheck();
  }

  private configurarFiltro(): void {
    this.dataSource.filterPredicate = (data: returnParcela, filter: string) => {
      if (!filter) return true;

      let searchTerms: { text?: string; status?: number | null };
      try {
        searchTerms = JSON.parse(filter);
      } catch {
        return true;
      }

      const matchTexto = searchTerms.text
        ? data.descricao?.toLowerCase().includes(searchTerms.text.toLowerCase())
        : true;

      const matchStatus = searchTerms.status !== null && searchTerms.status !== undefined
        ? data.status === searchTerms.status
        : true;

      return matchTexto && matchStatus;
    };
  }

  applyFilter(event: Event): void {
    this.filtroTexto = (event.target as HTMLInputElement).value.trim();
    this.atualizarFiltro();
  }

  applyStatusFilter(status: number | null): void {
    this.filtroStatus = status;
    this.atualizarFiltro();
  }

  private atualizarFiltro(): void {
    this.dataSource.filter = JSON.stringify({
      text: this.filtroTexto,
      status: this.filtroStatus
    });
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    const statusNumber = value === 'null' || value === '' ? null : Number(value);
    this.applyStatusFilter(statusNumber);
  }


  // Abre um pop-up para confirmar o pagamento\recebimento
  pagarConfirma(element: returnParcela, enterAnimationDuration: string, exitAnimationDuration: string): void {
    const tipo = Number(element.atribuicao);
    let configDialog;

    if (tipo === AtribuicaoEnum.Ganho) {
      configDialog = {
        titulo: 'Receita',
        mensagem: `Deseja confirmar recebimento de ${element.descricao} no valor de R$ ${element.valorParcela} ?`,
        textoConfirmar: 'Receber',
        textoCancelar: 'Não'
      };
    } else {
      configDialog = {
        titulo: 'Despesa',
        mensagem: `Deseja confirmar pagamento de ${element.descricao} no valor de R$ ${element.valorParcela} ?`,
        textoConfirmar: 'Pagar',
        textoCancelar: 'Não'
      };
    }
    
    const dialogRef = this.dialog.open(PopUp, {
      width: '400px',
      enterAnimationDuration,
      exitAnimationDuration,
      data: configDialog
    });

    dialogRef.afterClosed().subscribe((confirmado: boolean) => {
      if (confirmado) {
        this.onPagar.emit(element);
                
        //Tipo Snack = 'sucesso' | 'erro' | 'info' | 'despesa' | 'receita';
        const Snack = tipo === AtribuicaoEnum.Ganho ? 'receita' : 'despesa';
        const acao = tipo === AtribuicaoEnum.Ganho ? 'Recebimento' : 'Pagamento';

        this.abrirSnackBar(`${acao} confirmado com sucesso!`, 5, 'fa-solid fa-circle-check', Snack);
      }
      else {
        const Snack = tipo === AtribuicaoEnum.Ganho ? 'receita' : 'despesa';
        const acao = tipo === AtribuicaoEnum.Ganho ? 'Recebimento' : 'Pagamento';
        this.abrirSnackBar(`${acao} Não confirmado`, 5, 'fa-solid fa-ban', 'info')
      }
    });
  }


  // Chama o componente SnackBar
  abrirSnackBar(
    mensagem: string,
    duracaoSegundos: number = 3,
    icone: string = 'fa-solid fa-circle-check',
    tipo: TipoSnack = 'info'
  ): void {
    this.snackBar.openFromComponent(SnackBarCustomComponent, {
      duration: duracaoSegundos * 1000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      // Aplica a base + a variação de cor escolhida
      panelClass: ['snack-app-theme', `snack-${tipo}`],
      data: {
        mensagem,
        icone
      }
    });
  }






}