import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoneyCard } from '../../components/card/money-card/money-card';
import { MoneyChart } from '../../components/graphic/money-chart/money-chart';
import { DashboardServe, HistoricoFinanceiroAnual } from '../../service/Dashboard/dashboard-serve';
import { MoneyTable } from '../../components/Tabela/money-table/money-table';
import { ContaPendenteItem } from '../../models/contas-pendentes';

export interface DashboardCardItem {
  title: string;
  value: number;
  iconClass: string;
  typeClass: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MoneyCard, MoneyChart, MoneyTable],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardServe);
  private cdr = inject(ChangeDetectorRef);

  historicoFinanceiro?: HistoricoFinanceiroAnual;

  // Cards do Dashboard
  cardDividaPendente?: DashboardCardItem;
  cardValorReceber?: DashboardCardItem;
  cardValorSaldo?: DashboardCardItem;
  cardDeficit?: DashboardCardItem;

  contasPendentes: ContaPendenteItem[] = [];

  ngOnInit(): void {
    // 1. Carrega o histórico financeiro da API
    this.dashboardService.getHistoricoFinanceiroAnual(2026).subscribe({
      next: (resposta) => {
        if (resposta && resposta.length > 0) {
          const [dados] = resposta;

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

    // 2. Dívidas Pendentes
    this.dashboardService.getDividaPendente().subscribe({
      next: (resposta) => {
        if (resposta) {
          this.cardDividaPendente = {
            title: 'Conta a Pagar',
            value: resposta,
            iconClass: 'fa-solid fa-hand-holding-dollar', // Do seu mock
            typeClass: 'bills-to-pay', // Do seu mock
          };

          this.atualizarDeficit(); // Recalcula o déficit
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error('Erro ao carregar dívida pendente:', err);
      },
    });

    // 3. Valor a Receber
    this.dashboardService.getValorReceber().subscribe({
      next: (resposta) => {
        if (resposta) {
          this.cardValorReceber = {
            title: 'Receber',
            value: resposta,
            iconClass: 'fa-solid fa-coins', // Do seu mock
            typeClass: 'extra-income', // Do seu mock
          };
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error('Erro ao carregar valor a receber:', err);
      },
    });

    // 4. Valor em Saldo
    this.dashboardService.getValorSaldo().subscribe({
      next: (resposta: any) => {
        console.log('Dado vindo do backend:', resposta); // Imprime 660

        if (resposta) {
          this.cardValorSaldo = {
            title: 'Saldo',
            value: resposta, // Usa 'resposta' direto
            iconClass: 'fa-solid fa-wallet',
            typeClass: 'salary',
          };
          this.atualizarDeficit(); // Recalcula o déficit
          this.cdr.detectChanges();
        }
      },
      error: (err) => {
        console.error('Erro ao carregar valor em saldo:', err);
      },
    });

    // 5. Carrega a lista de contas pendentes unificadas
    this.dashboardService.getContasPendentesUnificadas().subscribe({
      next: (dados) => {
        this.contasPendentes = dados;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Erro ao carregar contas pendentes:', err);
      },
    });
  }

  // Função auxiliar para calcular o déficit quando ambas as APIs retornarem dados
  private atualizarDeficit(): void {
    if (this.cardValorSaldo && this.cardDividaPendente) {
      const valorCalculado = this.cardValorSaldo.value - this.cardDividaPendente.value;

      this.cardDeficit = {
        title: 'Déficit',
        value: valorCalculado,
        iconClass: 'fa-solid fa-credit-card',
        typeClass: 'deficit',
      };
    }
  }
}
