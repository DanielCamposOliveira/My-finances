import { Component } from '@angular/core';
import {MatTableDataSource, MatTableModule} from '@angular/material/table';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { StatusParcelaEnum } from '../../../enums/status-parcela-enum'
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';


export interface Parcela {
  id: number;
  numeroParcela: number;
  valorParcela: number;
  dataVencimento: string;
  status: number;
  lancamento_Descricao: string;
  lancamento_Id: number;
  atribuicao: number;
}

const ELEMENTDATA: Parcela[] = [
  // Notebook - 8 parcelas
  {
    id: 1,
    numeroParcela: 1,
    valorParcela: 142.86,
    dataVencimento: '2026-07-12T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 2,
    numeroParcela: 2,
    valorParcela: 142.86,
    dataVencimento: '2026-08-12T20:06:56Z',
    status: 2,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 3,
    numeroParcela: 3,
    valorParcela: 142.86,
    dataVencimento: '2026-09-12T20:06:56Z',
    status: 3,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 4,
    numeroParcela: 4,
    valorParcela: 142.86,
    dataVencimento: '2026-10-12T20:06:56Z',
    status: 4,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 5,
    numeroParcela: 5,
    valorParcela: 142.86,
    dataVencimento: '2026-11-12T20:06:56Z',
    status: 2,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 6,
    numeroParcela: 6,
    valorParcela: 142.86,
    dataVencimento: '2026-12-12T20:06:56Z',
    status: 3,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 7,
    numeroParcela: 7,
    valorParcela: 142.86,
    dataVencimento: '2027-01-12T20:06:56Z',
    status: 3,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },
  {
    id: 8,
    numeroParcela: 8,
    valorParcela: 142.86,
    dataVencimento: '2027-02-12T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Notebook',
    lancamento_Id: 1,
    atribuicao: 1
  },

  // Geladeira - 8 parcelas
  {
    id: 9,
    numeroParcela: 1,
    valorParcela: 287.50,
    dataVencimento: '2026-07-15T20:06:56Z',
    status: 4,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 10,
    numeroParcela: 2,
    valorParcela: 287.50,
    dataVencimento: '2026-08-15T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 11,
    numeroParcela: 3,
    valorParcela: 287.50,
    dataVencimento: '2026-09-15T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 12,
    numeroParcela: 4,
    valorParcela: 287.50,
    dataVencimento: '2026-10-15T20:06:56Z',
    status: 4,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 13,
    numeroParcela: 5,
    valorParcela: 287.50,
    dataVencimento: '2026-11-15T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 14,
    numeroParcela: 6,
    valorParcela: 287.50,
    dataVencimento: '2026-12-15T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 15,
    numeroParcela: 7,
    valorParcela: 287.50,
    dataVencimento: '2027-01-15T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },
  {
    id: 16,
    numeroParcela: 8,
    valorParcela: 287.50,
    dataVencimento: '2027-02-15T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Geladeira',
    lancamento_Id: 2,
    atribuicao: 1
  },

  // TV - 6 parcelas
  {
    id: 17,
    numeroParcela: 1,
    valorParcela: 199.90,
    dataVencimento: '2026-07-20T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'TV',
    lancamento_Id: 3,
    atribuicao: 1
  },
  {
    id: 18,
    numeroParcela: 2,
    valorParcela: 199.90,
    dataVencimento: '2026-08-20T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'TV',
    lancamento_Id: 3,
    atribuicao: 1
  },
  {
    id: 19,
    numeroParcela: 3,
    valorParcela: 199.90,
    dataVencimento: '2026-09-20T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'TV',
    lancamento_Id: 3,
    atribuicao: 1
  },
  {
    id: 20,
    numeroParcela: 4,
    valorParcela: 199.90,
    dataVencimento: '2026-10-20T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'TV',
    lancamento_Id: 3,
    atribuicao: 1
  },
  {
    id: 21,
    numeroParcela: 5,
    valorParcela: 199.90,
    dataVencimento: '2026-11-20T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'TV',
    lancamento_Id: 3,
    atribuicao: 1
  },
  {
    id: 22,
    numeroParcela: 6,
    valorParcela: 199.90,
    dataVencimento: '2026-12-20T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'TV',
    lancamento_Id: 3,
    atribuicao: 1
  },

  // Telefone - 5 parcelas
  {
    id: 23,
    numeroParcela: 1,
    valorParcela: 89.90,
    dataVencimento: '2026-08-05T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Telefone',
    lancamento_Id: 4,
    atribuicao: 1
  },
  {
    id: 24,
    numeroParcela: 2,
    valorParcela: 89.90,
    dataVencimento: '2026-09-05T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Telefone',
    lancamento_Id: 4,
    atribuicao: 1
  },
  {
    id: 25,
    numeroParcela: 3,
    valorParcela: 89.90,
    dataVencimento: '2026-10-05T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Telefone',
    lancamento_Id: 4,
    atribuicao: 1
  },
  {
    id: 26,
    numeroParcela: 4,
    valorParcela: 89.90,
    dataVencimento: '2026-11-05T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Telefone',
    lancamento_Id: 4,
    atribuicao: 1
  },
  {
    id: 27,
    numeroParcela: 5,
    valorParcela: 89.90,
    dataVencimento: '2026-12-05T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Telefone',
    lancamento_Id: 4,
    atribuicao: 1
  },

  // Sofá - 10 parcelas
  {
    id: 28,
    numeroParcela: 1,
    valorParcela: 159.90,
    dataVencimento: '2026-08-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 29,
    numeroParcela: 2,
    valorParcela: 159.90,
    dataVencimento: '2026-09-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 30,
    numeroParcela: 3,
    valorParcela: 159.90,
    dataVencimento: '2026-10-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 31,
    numeroParcela: 4,
    valorParcela: 159.90,
    dataVencimento: '2026-11-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 32,
    numeroParcela: 5,
    valorParcela: 238.90,
    dataVencimento: '2026-12-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 33,
    numeroParcela: 6,
    valorParcela: 159.90,
    dataVencimento: '2027-01-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 34,
    numeroParcela: 7,
    valorParcela: 159.90,
    dataVencimento: '2027-02-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 35,
    numeroParcela: 8,
    valorParcela: 159.90,
    dataVencimento: '2027-03-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 36,
    numeroParcela: 9,
    valorParcela: 159.90,
    dataVencimento: '2027-04-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  },
  {
    id: 37,
    numeroParcela: 10,
    valorParcela: 159.90,
    dataVencimento: '2027-05-10T20:06:56Z',
    status: 1,
    lancamento_Descricao: 'Sofá',
    lancamento_Id: 5,
    atribuicao: 1
  }
];
  

@Component({
  selector: 'app-TableContasFixa',
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
  templateUrl: './TableContasFixa.html',
  styleUrl: './TableContasFixa.scss',
})
export class TableContasFixa {
  
  // Disponibiliza o enum para o template
  readonly StatusParcelaEnum = StatusParcelaEnum;
  
  displayedColumns: string[] = [
    'id',
    'numeroParcela',
    'lancamento_Descricao',
    'valorParcela',
    'dataVencimento',
    'status',
    'acoes'
  ];

  dataSource = new MatTableDataSource<Parcela>(ELEMENTDATA);

  // Extrai as opções do Enum ignorando as chaves reversas numéricas
  statusOptions = Object.keys(StatusParcelaEnum)
    .filter(key => isNaN(Number(key)))
    .map(key => ({
      label: key,
      value: StatusParcelaEnum[key as keyof typeof StatusParcelaEnum] as number
    }));

  filtroTexto: string = '';
  filtroStatus: number | null = null;

  ngOnInit(): void { 
    // Configura o predicado customizado que avalia texto e status ao mesmo tempo
    this.dataSource.filterPredicate = (data: Parcela, filter: string) => {
      const searchTerms = JSON.parse(filter);

      const matchTexto = searchTerms.text
        ? data.lancamento_Descricao.toLowerCase().includes(searchTerms.text.toLowerCase())
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
  
  abrirModalCadastro(): void {}

  pagar(element: Parcela): void {}

  editar(element: Parcela): void {}
}
