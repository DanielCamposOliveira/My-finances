import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject, ChangeDetectorRef, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';


import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';

import { PopUp } from '../../Dialog/pop-up/pop-up';
import { MatDialog } from '@angular/material/dialog';

import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarCustomComponent } from '../../SnackBar/snack-bar-info/snack-bar-info';

import { ContaFixa } from "../../../models/canta-fixa";
registerLocaleData(localePt);
export type TipoSnack = 'sucesso' | 'erro' | 'info' | 'despesa' | 'receita';


@Component({
  selector: 'app-TableContafixa',
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
  templateUrl: './TableContafixa.component.html',
  styleUrl: './TableContafixa.component.scss',
})

// ...
export class TableContafixa implements OnInit, OnChanges {
  private cdr = inject(ChangeDetectorRef);

  @Input() titulo: string = 'ContaFixas';
  @Input() ContaFixas: ContaFixa[] = [];
  @Output() OpenPageCadastro = new EventEmitter<void>();
  //@Output() AlteraStatusConta = new EventEmitter<{ conta: ContaFixa; NewStatus: boolean }>();
  @Output() AlteraStatusConta = new EventEmitter<{ element: ContaFixa; ativo: boolean }>();


  displayedColumns: string[] = [
    'id',
    'descricao',
    'valorBase',
    'diaVencimento',
    'status',
    'acoes'
  ];
  
  dataSource = new MatTableDataSource<ContaFixa>([]);
  
  filtroTexto: string = '';
  filtroStatus: boolean | null = null;

  readonly dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  ngOnInit(): void {
    this.atualizarDataSource(this.ContaFixas || []);
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Note a correção para 'ContaFixas' (com 's')
    if (changes['ContaFixas'] && this.ContaFixas) {
      this.atualizarDataSource(this.ContaFixas);
    }
  }

  private atualizarDataSource(dados: ContaFixa[]): void {
    this.dataSource = new MatTableDataSource<ContaFixa>(dados);
    this.configurarFiltro();
    this.atualizarFiltro();
    this.cdr.markForCheck();
  }

  private configurarFiltro(): void {
    this.dataSource.filterPredicate = (data: ContaFixa, filter: string) => {
      if (!filter) return true;

      let searchTerms: { text?: string; status?: boolean | null; num?: number | null };
      try {
        searchTerms = JSON.parse(filter);
      } catch {
        return true;
      }

      // 1. Tratamento da busca por texto em múltiplos campos
      const textoFiltro = searchTerms.text?.toLowerCase().trim() || '';
      let matchTexto = true;

      if (textoFiltro) {
        const id = String(data.id || '');
        const descricao = (data.descricao || '').toLowerCase();
        const valorBase = String(data.valorBase || '');
        const diaVencimento = String(data.diaVencimento || '');
        const categoriaId = String(data.categoriaId || '');

        // Bate com qualquer um dos campos textuais/numéricos
        matchTexto = (
          descricao.includes(textoFiltro) ||
          valorBase.includes(textoFiltro) ||
          diaVencimento.includes(textoFiltro) ||
          id.includes(textoFiltro) ||
          categoriaId.includes(textoFiltro)
        );
      }

      // 2. Tratamento do filtro de Status (ativo: true / false)
      const matchStatus = searchTerms.status !== null && searchTerms.status !== undefined
        ? data.ativo === searchTerms.status
        : true;

      // 3. (Opcional) Tratamento do campo 'num' caso seja usado (ex: dia de vencimento exato)
      const matchNum = searchTerms.num !== null && searchTerms.num !== undefined
        ? data.diaVencimento === searchTerms.num
        : true;

      // Retorna true somente se atender a todos os grupos de critérios ativos
      return matchTexto && matchStatus && matchNum;
    };
  }
  
  applyFilter(event: Event): void {
    this.filtroTexto = (event.target as HTMLInputElement).value.trim();
    this.atualizarFiltro();
  }

  applyStatusFilter(status: boolean | null): void {
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
    const statusBool = value === 'null' || value === '' ? null : value === 'true';
    this.applyStatusFilter(statusBool);
  }

  abrirPageCadastro() {
    this.OpenPageCadastro.emit();
  }

  // muda o valor do ATIVO da conta
  updateStatusContaFixas(element: ContaFixa) {
    const novoStatus = !element.ativo;
    console.log(novoStatus);

    this.AlteraStatusConta.emit({
      element,
      ativo: novoStatus
    });
  }


  ConfirmUpdateStatusContaFixa(element: ContaFixa, enterAnimationDuration: string, exitAnimationDuration: string): void {
    const novoStatus = !element.ativo;
    
    let configDialog;
    if (element.ativo) {
      configDialog = {
        titulo: 'Conta Fixa',
        mensagem: `Deseja Desativar Conta - ${element.descricao} no valor de R$ ${element.valorBase} ?`,
        textoConfirmar: 'Desativar',
        textoCancelar: 'Cancelar'
      };
    }
    else {
      configDialog = {
        titulo: 'Conta Fixa',
        mensagem: `Deseja Ativar Conta - ${element.descricao} no valor de R$ ${element.valorBase} ?`,
        textoConfirmar: 'Ativar',
        textoCancelar: 'Cancelar'
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
        this.AlteraStatusConta.emit({
          element,
          ativo: novoStatus
        });

        this.abrirSnackBar(`confirmado com sucesso!`, 5, 'fa-solid fa-circle-check', 'despesa');

      }
      else {
        this.abrirSnackBar(`Não confirmado`, 5, 'fa-solid fa-ban', 'info');
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
