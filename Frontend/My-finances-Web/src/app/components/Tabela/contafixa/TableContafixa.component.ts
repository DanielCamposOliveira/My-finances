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

import { AtribuicaoEnum } from '../../../enums/atribuicao-enum';

import { ContaFixa } from "../../../models/canta-fixa";

registerLocaleData(localePt);

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

  displayedColumns: string[] = [
    'id',
    'descricao',
    'valorBase',
    'diaVencimento',
    'status'
  ];
  
  dataSource = new MatTableDataSource<ContaFixa>([]);
  
  filtroTexto: string = '';
  filtroStatus: boolean | null = null;

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
  
      let searchTerms: { text?: string; status?: boolean | null };
      try {
        searchTerms = JSON.parse(filter);
      } catch {
        return true;
      }
  
      const matchTexto = searchTerms.text
        ? data.descricao?.toLowerCase().includes(searchTerms.text.toLowerCase())
        : true;
  
      const matchStatus = searchTerms.status !== null && searchTerms.status !== undefined
        ? data.ativo === searchTerms.status
        : true;
  
      return matchTexto && matchStatus;
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
}
