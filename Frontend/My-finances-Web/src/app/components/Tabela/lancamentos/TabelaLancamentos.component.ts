import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject, ChangeDetectorRef, signal, input } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';


import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';

import {LancamentoResponseList} from '../../../models/lancamentos.model'
registerLocaleData(localePt);

@Component({
  selector: 'app-TabelaLancamentos',
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
  templateUrl: './TabelaLancamentos.component.html',
  styleUrl: './TabelaLancamentos.component.scss',
})
export class TabelaLancamentos implements OnInit, OnChanges {
  private cdr = inject(ChangeDetectorRef);

  @Input() titulo: string = 'Tabela';
  @Input() Lancamentos: LancamentoResponseList[] = [];
  @Output() OpenPageCadastro = new EventEmitter<void>();


  displayedColumns: string[] = [
    'id',
    'Descricao',
    'valorTotal',
    'valorParcela',
    'ParcelasRestante',
    'DataVencimentoUltimaParcela',
    'CategoriaNome'
  ];

  dataSource = new MatTableDataSource<LancamentoResponseList>([]);
  filtroTexto: string = '';

  ngOnInit(): void {
     this.atualizarDataSource(this.Lancamentos || []);
  }

  ngOnChanges(changes: SimpleChanges): void { 
        if (changes['Lancamentos'] && this.Lancamentos) {
      this.atualizarDataSource(this.Lancamentos);
    }
  }


  private atualizarDataSource(dados: LancamentoResponseList[]): void {

    //this.dataSource = new MatTableDataSource<LancamentoResponseList>(dados);
    this.dataSource.data = dados;
    this.configurarFiltro();
    this.atualizarFiltro();
    this.cdr.markForCheck();      
  }

private configurarFiltro(): void {
  // Sobrescreve a função padrão de filtro do MatTableDataSource.
  // Ela é executada para cada item (linha) da tabela sempre que 'dataSource.filter' muda.
  this.dataSource.filterPredicate = (data: LancamentoResponseList, filter: string) => {
    
    // Passo 1: Se o filtro estiver vazio ou nulo, exibe todas as linhas.
    if (!filter) return true;

    // Passo 2: Como enviamos os critérios via JSON stringify, precisamos desserializar.
    let searchTerms: { text?: string | null };
    try {
      searchTerms = JSON.parse(filter);
    } catch {
      // Se a string não for um JSON válido, evita quebrar a tela e exibe a linha.
      return true;
    }

    // Passo 3: Limpa espaços extras e padroniza a busca em letras minúsculas (case-insensitive).
    const textoFiltro = searchTerms.text?.toLowerCase().trim() || '';

    // Se o usuário não digitou nada no campo de texto, mantém o registro visível.
    if (!textoFiltro) return true;

    // Passo 4: Normalização dos dados da linha.
    // - Usa .toLowerCase() para garantir que "CARRO" ache "carro".
    // - Usa (data as any) como fallback para evitar quebras caso o backend varie entre PascalCase e camelCase.
    // - Converte números e formatos mistos para string via String() para permitir busca parcial.
    const descricao = (data.Descricao || (data as any).descricao || '').toLowerCase();
    const categoria = (data.CategoriaNome || (data as any).categoriaNome || '').toLowerCase();
    const parcelas = String(data.ParcelasRestante || (data as any).parcelasRestante || '').toLowerCase();
    const valorTotal = String(data.valorTotal || '');

    // Passo 5: Avaliação ampla (OU lógico - ||).
    // Se o texto digitado estiver contido em QUALQUER um desses campos, retorna true e exibe a linha.
    return (
      descricao.includes(textoFiltro) ||
      categoria.includes(textoFiltro) ||  
      parcelas.includes(textoFiltro) ||
      valorTotal.includes(textoFiltro)
    );
  };
}
  
    applyFilter(event: Event): void {
    this.filtroTexto = (event.target as HTMLInputElement).value.trim();
    this.atualizarFiltro();
    }
  
    private atualizarFiltro(): void {
    this.dataSource.filter = JSON.stringify({
      text: this.filtroTexto,
      
    });
    }
  
    onStatusChange(event: Event): void {
      const value = (event.target as HTMLSelectElement).value;
      this.atualizarFiltro();
  }

  abrirPageCadastro() {
    this.OpenPageCadastro.emit();
  }

}
