import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, map } from 'rxjs';
import { ContaPendenteItem } from '../../models/contas-pendentes';
// Interfaces para os dados do histórico financeiro
export interface ChartSeries {
  type?: string;
  name: string;
  color: string;
  data: number[];
}

// Interface para o histórico financeiro anual
export interface HistoricoFinanceiroAnual {
  chartCategories: string[];
  chartSeries: ChartSeries[];
}

@Injectable({
  providedIn: 'root',
})
export class DashboardServe {
  private http = inject(HttpClient);

  // URLs das APIs
  private readonly HistoricoFinanceiroAnual =
    'http://localhost:5000/api/v1/HistoricoFinanceiroAnual';
  private readonly baseUrlDividaPendente =
    'http://localhost:5000/api/v1/Consulta/Dividas/pendentes';
  private readonly baseUrlValorReceber = 'http://localhost:5000/api/v1/Consulta/Valores/receber';
  private readonly baseUrlValorSaldo = 'http://localhost:5000/api/v1/Consulta/Valores/saldo';

  private readonly baseUrlContasFixas =
    'http://localhost:5000/api/v1/ContasFixas/parcela/pendentes';
  private readonly baseUrlLancamentos =
    'http://localhost:5000/api/v1/lancamentos/parcela/pendentes';

  // Métodos para consumir as APIs
  getHistoricoFinanceiroAnual(ano: number = 2026): Observable<HistoricoFinanceiroAnual[]> {
    return this.http.get<HistoricoFinanceiroAnual[]>(`${this.HistoricoFinanceiroAnual}/${ano}`);
  }

  getDividaPendente(): Observable<number> {
    return this.http.get<number>(`${this.baseUrlDividaPendente}`);
  }

  getValorReceber(): Observable<number> {
    return this.http.get<number>(`${this.baseUrlValorReceber}`);
  }

  getValorSaldo(): Observable<number> {
    return this.http.get<number>(`${this.baseUrlValorSaldo}`);
  }

  getContasPendentesUnificadas(): Observable<ContaPendenteItem[]> {
    return forkJoin({
      contasFixas: this.http.get<any[]>(this.baseUrlContasFixas),
      lancamentos: this.http.get<any[]>(this.baseUrlLancamentos),
    }).pipe(
      map(({ contasFixas, lancamentos }) => {
        // Normaliza as Contas Fixas
        const fixasMapeadas: ContaPendenteItem[] = contasFixas.map((item) => ({
          id: item.id,
          origemId: item.contaFixaId,
          descricao: item.descricao,
          valorParcela: item.valorParcela,
          dataVencimento: item.dataVencimento,
          dataPagamento: item.dataPagamento,
          status: item.status, // Número vindo da API
          tipo: 'CONTA_FIXA',
        }));

        // Normaliza os Lançamentos
        const lancamentosMapeados: ContaPendenteItem[] = lancamentos.map((item) => ({
          id: item.id,
          origemId: item.lancamento_Id,
          descricao: item.lancamento_Descricao,
          valorParcela: item.valorParcela,
          dataVencimento: item.dataVencimento,
          numeroParcela: item.numeroParcela,
          status: item.status, // Número vindo da API
          tipo: 'LANCAMENTO',
        }));

        // Une os dois arrays
        return [...fixasMapeadas, ...lancamentosMapeados];
      }),
    );
  }
}
