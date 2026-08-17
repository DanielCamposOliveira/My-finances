import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContaPendenteItemModel } from '../../../models/InterfaceModel';
import { StatusParcelaEnum } from '../../../enums/status-parcela-enum';


@Component({
  selector: 'app-money-table',
  imports: [CommonModule],
  templateUrl: './money-table.html',
  styleUrl: './money-table.scss',
})
export class MoneyTable {
  // Recebe a lista direta vinda do forkJoin/API
  @Input() links: ContaPendenteItemModel[] = [];

  
  // Mapeia o número retornado para o texto na tela
  getStatusLabel(status: number): string {
    switch (status) {
      case StatusParcelaEnum.Aberto:
        return 'Aberto';
      case StatusParcelaEnum.Pago:
        return 'Pago';
      case StatusParcelaEnum.Atrasado:
        return 'Atrasado';
      case StatusParcelaEnum.Cancelado:
        return 'Cancelado';
      default:
        return 'Pendente';
    }
  }

  getAtribuicaoLabel(atribuicao: number): string {
    switch (atribuicao) {
      case 1:
        return 'Despesa';
      case 2:
        return 'Ganho';
      default:
        return 'Desconhecido';
    }
  }

getAtribuicaoClass(atribuicao: number): string {
  switch (atribuicao) {
    case 1:
      return 'badge-atribuicao-despesa';
    case 2:
      return 'badge-atribuicao-receita';
    default:
      return 'badge-atribuicao-desconhecido';
  }
}

getAtribuicaoIcon(atribuicao: number): string {
  switch (atribuicao) {
    case 1:
      return 'fa-solid fa-wallet';
    case 2:
      return 'fa-solid fa-coins';
    default:
      return 'fa-solid fa-circle-question';
  }
}


  // Mapeia o número retornado para a classe CSS da badge
  getStatusClass(status: number): string {
    switch (status) {
      case StatusParcelaEnum.Aberto:
        return 'badge-status-aberto';
      case StatusParcelaEnum.Pago:
        return 'badge-status-pago';
      case StatusParcelaEnum.Atrasado:
        return 'badge-status-atrasado';
      case StatusParcelaEnum.Cancelado:
        return 'badge-status-cancelado';
      default:
        return 'badge-status-aberto';
    }
  }
}
