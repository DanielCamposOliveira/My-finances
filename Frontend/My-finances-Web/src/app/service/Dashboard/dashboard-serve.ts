import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChartSeries {
  type?: string;
  name: string;
  color: string;
  data: number[];
}

export interface HistoricoFinanceiroAnual {
  chartCategories: string[];
  chartSeries: ChartSeries[];
}

@Injectable({
  providedIn: 'root',
})
export class DashboardServe {
  private http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api/v1/HistoricoFinanceiroAnual';

  getHistoricoFinanceiroAnual(ano: number = 2026): Observable<HistoricoFinanceiroAnual[]> {
    return this.http.get<HistoricoFinanceiroAnual[]>(`${this.baseUrl}/${ano}`);
  }
}
