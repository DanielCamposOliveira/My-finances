import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoneyCard } from '../../components/card/money-card/money-card';
import { MoneyChart } from '../../components/graphic/money-chart/money-chart';
import { ServiceData, DashboardData } from '../../service/service.data';
import { DashboardServe, HistoricoFinanceiroAnual } from '../../service/Dashboard/dashboard-serve';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MoneyCard, MoneyChart],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private serviceData = inject(ServiceData);
  private dashboardService = inject(DashboardServe);
  private cdr = inject(ChangeDetectorRef); // Injeta o detector de mudanças

  dashboardData?: DashboardData;
  historicoFinanceiro?: HistoricoFinanceiroAnual;

  ngOnInit(): void {
    // 1. Carrega os cards do mock
    this.serviceData.getDashboardData().subscribe({
      next: (data) => {
        this.dashboardData = data;
        this.cdr.detectChanges();
      },
    });

    // 2. Carrega o histórico financeiro da API
    this.dashboardService.getHistoricoFinanceiroAnual(2026).subscribe({
      next: (resposta) => {
        if (resposta && resposta.length > 0) {
          const [dados] = resposta;

          console.log('Dados do histórico financeiro:', dados);

          this.historicoFinanceiro = {
            chartCategories: dados.chartCategories,
            chartSeries: dados.chartSeries,
          };

          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error('Erro ao carregar histórico financeiro:', err);
      },
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  }
}
