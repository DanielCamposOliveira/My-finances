import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';

import { StatusParcelaEnum } from '../../../enums/status-parcela-enum';
import { returnParcela } from "../../../models/parcela-model";

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
  
  displayedColumns: string[] = [
    'id',
    'numeroParcela',
    'descricao',
    'valorParcela',
    'dataVencimento',
    'status',
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

  pagar(element: returnParcela): void {
    this.onPagar.emit(element);
  }
}