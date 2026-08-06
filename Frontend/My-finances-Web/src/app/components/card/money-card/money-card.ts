import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardCardItemModel } from '../../../models/InterfaceModel';

@Component({
  selector: 'app-money-card',
  imports: [CommonModule],
  templateUrl: './money-card.html',
  styleUrl: './money-card.scss',
})
export class MoneyCard {
  // Recebe os dados do card via Input
  @Input() card!: DashboardCardItemModel;
}
