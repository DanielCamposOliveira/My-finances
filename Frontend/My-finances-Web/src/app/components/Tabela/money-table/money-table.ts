import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContaPendenteItem, StatusParcela } from '../../../models/contas-pendentes';

@Component({
  selector: 'app-money-table',
  imports: [CommonModule],
  templateUrl: './money-table.html',
  styleUrl: './money-table.scss',
})
export class MoneyTable {
  // Recebe a lista direta vinda do forkJoin/API
  @Input() links: ContaPendenteItem[] = [];

  // Mapeia o número retornado para o texto na tela
  getStatusLabel(status: number): string {
    switch (status) {
      case StatusParcela.Aberto:
        return 'Aberto';
      case StatusParcela.Pago:
        return 'Pago';
      case StatusParcela.Atrasado:
        return 'Atrasado';
      case StatusParcela.Cancelado:
        return 'Cancelado';
      default:
        return 'Pendente';
    }
  }

  // Mapeia o número retornado para a classe CSS da badge
  getStatusClass(status: number): string {
    switch (status) {
      case StatusParcela.Aberto:
        return 'badge-status-aberto';
      case StatusParcela.Pago:
        return 'badge-status-pago';
      case StatusParcela.Atrasado:
        return 'badge-status-atrasado';
      case StatusParcela.Cancelado:
        return 'badge-status-cancelado';
      default:
        return 'badge-status-aberto';
    }
  }
}
