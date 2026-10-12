import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject, ChangeDetectorRef, signal } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';

//
import { PopUp } from '../../Dialog/pop-up/pop-up'

//
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarCustomComponent } from '../../SnackBar/snack-bar-info/snack-bar-info';

//
import { DialogEditarValorData, DialogInputComponent } from '../../Dialog/dialog-input.component/dialog-input.component';

import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';

import { AtribuicaoEnum } from '../../../enums/atribuicao-enum';
import { StatusParcelaEnum, StatusParcelaFiltroEnum } from '../../../enums/status-parcela-enum';
import { returnParcela } from "../../../models/parcela-model";


import {ContaFixaValorParcelaModel} from '../../../models/canta-fixa'
import {
  MatDialog,
} from '@angular/material/dialog';


registerLocaleData(localePt);

export type TipoSnack = 'sucesso' | 'erro' | 'info' | 'despesa' | 'receita';

@Component({
  selector: 'app-TableParcela',
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

  @Output() onValor = new EventEmitter<ContaFixaValorParcelaModel>();


  readonly statusParcelaFiltroEnum = StatusParcelaFiltroEnum;
  readonly StatusParcelaEnum = StatusParcelaEnum;
  readonly AtribuicaoEnum = AtribuicaoEnum;

  filtroMesAtual: boolean = false;

  readonly dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  
  valorTotalSelecionado: number = 0; // Propriedade que armazena a soma total dos valores das parcelas selecionadas  
  itensSelecionados = new Set<number>(); // Utiliza um Set para armazenar os IDs dos itens selecionados (evita duplicatas e facilita buscas rápidas)

  
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

  statusOptions = Object.keys(this.statusParcelaFiltroEnum)
    .filter(key => isNaN(Number(key)))
    .map(key => ({
      label: key,
      value: this.statusParcelaFiltroEnum[key as keyof typeof this.statusParcelaFiltroEnum] as number
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

      let searchTerms: { text?: string; status?: number | null; mesAtual?: boolean; };
      try {
        searchTerms = JSON.parse(filter);
      } catch {
        return true;
      }
      // Filtro de Texto
      const matchTexto = searchTerms.text
        ? data.descricao?.toLowerCase().includes(searchTerms.text.toLowerCase())
        : true;

      // Filtro de Status
      const matchStatus = searchTerms.status !== null && searchTerms.status !== undefined
        ? data.status === searchTerms.status
        : true;


      
      // Filtro do Mês Atual
      let matchMes = true;
      if (searchTerms.mesAtual) {
        if (data.dataVencimento) {
          // O formato "2027-02-05T03:00:00Z" é lido perfeitamente pelo new Date()
          const dataParcela = new Date(data.dataVencimento);
          const hoje = new Date();
          
          // Verifica se a data é válida e compara o mês e o ano
          if (!isNaN(dataParcela.getTime())) {
            matchMes = 
              dataParcela.getMonth() === hoje.getMonth() && 
              dataParcela.getFullYear() === hoje.getFullYear();
          } else {
            matchMes = false;
          }
        } else {
          matchMes = false;
        }
      }
      
      return matchTexto && matchStatus && matchMes;
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



  // Receber o evento do select 
onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    if (value === 'mes_atual') {
      this.filtroMesAtual = true;
      this.filtroStatus = null;
    } else {
      this.filtroMesAtual = false;
      this.filtroStatus = value === 'null' || value === '' ? null : Number(value);
    }

    this.atualizarFiltro();
  }

  private atualizarFiltro(): void {
    this.dataSource.filter = JSON.stringify({
      text: this.filtroTexto,
      status: this.filtroStatus,
      mesAtual: this.filtroMesAtual
    });
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

  // Adiciona o iten para somar
  // Método "onToggleSelecao" disparado ao marcar ou desmarcar o checkbox de uma parcela.
  // @param element Objeto da parcela correspondente à linha atual.
  // @param event Evento do DOM contendo o estado atual do checkbox (checked: true/false).
  onToggleSelecao(element: returnParcela, event: any): void {
    const checked = event.target.checked;
    
    // Adiciona o ID do item ao conjunto se marcado, ou remove se desmarcado
    if (checked) {
      this.itensSelecionados.add(element.id);      
    } else {
      this.itensSelecionados.delete(element.id);
    }

    this.calcularValorTotalSelecionado();
  }
  

  // Calculadora de somar os valores selecionados
  //Método responsável por filtrar todas as parcelas atuais, identificar quais, possuem o ID presente no Set de selecionados e somar seus respectivos valores.
  private calcularValorTotalSelecionado(): void {
    this.valorTotalSelecionado = this.parcelas
      .filter(p => this.itensSelecionados.has(p.id)
        &&
        p.atribuicao === AtribuicaoEnum.Despesa
    ) // Filtra apenas os itens cujos IDs estão selecionados
      .reduce((acc, curr) => acc + Number(curr.valorParcela || 0), 0); // Soma os valores de cada parcela
  
    // Força a detecção de mudanças para atualizar a tela imediatamente
    this.cdr.markForCheck();
  }



  abrirModalEditar(element: returnParcela): void {
    const valorParcela = Number(element.valorParcela);
    const descricao = String(element.descricao);
    const tipo = Number(element.atribuicao);

    const TipoAtribuicao = tipo === AtribuicaoEnum.Ganho ? 'recebimento' : 'despesa';
    
    const configData: DialogEditarValorData = {
      titulo: descricao,
      label: `Valor ${TipoAtribuicao}`,
      valorInicial: valorParcela,
      textoConfirmar: 'Atualizar',
      textoCancelar: 'Cancelar'
    };

    const dialogRef = this.dialog.open(DialogInputComponent, {
      width: '400px',
      panelClass: 'custom-dialog-container',
      data: configData
    });

    dialogRef.afterClosed().subscribe((novoValor: Number) => {
      // Se não for cancelado/undefined e houver alteração
      if (novoValor !== undefined) {
        const valor = Number(novoValor);

        const pacote: ContaFixaValorParcelaModel = {
          parcelaId: element.id,
          valorParcela: valor
        }

        this.onValor.emit(pacote);

        this.abrirSnackBar(`Valor ${TipoAtribuicao} alterado`, 5, 'fa-solid fa-ban', 'info')
      }
      else {
        this.abrirSnackBar(`Valor ${TipoAtribuicao} não alterado`, 5, 'fa-solid fa-ban', 'info')
      }
    });
  }


}