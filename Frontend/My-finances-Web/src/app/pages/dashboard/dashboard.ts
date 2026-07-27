import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MoneyCard } from '../../components/card/money-card/money-card';
import { MoneyChart } from '../../components/graphic/money-chart/money-chart';
import { ServiceData, DashboardData } from '../../service/service.data';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MoneyCard, MoneyChart],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {

  private serviceData = inject(ServiceData);
  
  dashboardData?: DashboardData;

  ngOnInit(): void {
    // Solicita os dados mockados do service
    this.serviceData.getDashboardData().subscribe({
      next: (data) => {
        this.dashboardData = data;
      }
    });
  }

  // Método auxiliar para formatação de moeda BRL
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}
