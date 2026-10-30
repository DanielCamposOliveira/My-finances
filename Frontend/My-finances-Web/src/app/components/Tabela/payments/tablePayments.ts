import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';


import { AtribuicaoEnum } from '../../../enums/atribuicao-enum';
import { StatusParcelaEnum } from '../../../enums/status-parcela-enum';
import { returnParcela } from '../../../models/parcela.model';

registerLocaleData(localePt);

@Component({
  selector: 'app-tablePayments',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './tablePayments.html',
  styleUrl: './tablePayments.scss',
})
export class TablePayments {
  private cdr = inject(ChangeDetectorRef);
  @Input() titulo: string = 'Comprovantes de Pagamento';
  @Input() parcelas: returnParcela[] = [];

// Emite o evento ao clicar no botão de upload (quando ProofPayment é null/vazio)
  @Output() onUploadComprovante = new EventEmitter<returnParcela>();

  // Emite o evento para abrir/baixar o arquivo (quando já possui ProofPayment)
  @Output() onVisualizarComprovante = new EventEmitter<returnParcela>();

  readonly StatusParcelaEnum = StatusParcelaEnum;
  readonly AtribuicaoEnum = AtribuicaoEnum;

  valorTotalSelecionado: number = 0;
  itensSelecionados = new Set<number>();


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
  filtroTexto: string = '';

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
    this.aplicarFiltro();
    this.cdr.markForCheck();
  }

private configurarFiltro(): void {
  this.dataSource.filterPredicate = (data: returnParcela, filter: string) => {
    // Se não há filtro ou a busca estiver vazia, exibe todos os registros
    if (!filter) return true;

    const termo = filter.trim().toLowerCase();
    if (!termo) return true;

    // 1. Descrição
    const matchDescricao = data.descricao
      ? data.descricao.toLowerCase().includes(termo)
      : false;

    // 2. Valor da Parcela (ex: busca por "150" ou "150.50")
    const matchValor = data.valorParcela !== null && data.valorParcela !== undefined
      ? String(data.valorParcela).toLowerCase().includes(termo)
      : false;

    // 3. Número da Parcela (ex: busca por "1", "2")
    const matchNumeroParcela = data.numeroParcela !== null && data.numeroParcela !== undefined
      ? String(data.numeroParcela).toLowerCase().includes(termo)
      : false;

    // 4. Data de Vencimento (testa tanto ISO quanto formato legível dd/mm/yyyy)
    let matchVencimento = false;
    if (data.dataVencimento) {
      const dataIso = data.dataVencimento.toLowerCase();
      
      // Formata para dd/mm/yyyy para permitir buscar digitando datas comuns
      const dataObj = new Date(data.dataVencimento);
      const dataFormatada = !isNaN(dataObj.getTime())
        ? dataObj.toLocaleDateString('pt-BR')
        : '';

      matchVencimento = dataIso.includes(termo) || dataFormatada.includes(termo);
    }

    // Retorna true se encontrar em qualquer um dos campos
    return matchDescricao || matchValor || matchNumeroParcela || matchVencimento;
  };
}

applyFilter(event: Event): void {
  const inputElement = event.target as HTMLInputElement;
  this.dataSource.filter = inputElement.value.trim().toLowerCase();
}

  private aplicarFiltro(): void {
    this.dataSource.filter = this.filtroTexto;
  }

  // Ação dinâmica do botão de comprovante
  gerenciarComprovante(element: returnParcela): void {
    if (element.proofPayment) {
      this.onVisualizarComprovante.emit(element);
    } else {
      this.onUploadComprovante.emit(element);
    }
  }






  
}
