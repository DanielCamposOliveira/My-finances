import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
// Importa a interface DashboardCardItem do arquivo dashboard.ts
import { DashboardCardItem } from '../../../pages/dashboard/dashboard';

@Component({
  selector: 'app-money-card',

  imports: [CommonModule],
  templateUrl: './money-card.html',
  styleUrl: './money-card.scss',
})
export class MoneyCard {
  // Recebe os dados do card via Input
  @Input() card!: DashboardCardItem;
}
