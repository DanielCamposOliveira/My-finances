import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
}
