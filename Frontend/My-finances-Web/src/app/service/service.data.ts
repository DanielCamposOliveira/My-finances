import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs'; // <--- Import importante!

export interface CardData {
  title: string;
  value: number;
  iconClass: string;
  typeClass: string;
}

export interface ChartSeries {
  type?: string;
  name: string;
  color: string;
  data: number[];
}

export interface DashboardData {
  cards: CardData[];
  chartCategories: string[];
  chartSeries: ChartSeries[];
}

@Injectable({
  providedIn: 'root',
})
export class ServiceData {
  getDashboardData(): Observable<DashboardData> {
    const mockData: DashboardData = {
      cards: [
        { title: 'Saldo', value: 3475, iconClass: 'fa-solid fa-wallet', typeClass: 'salary' },
        { title: 'Receber', value: 80, iconClass: 'fa-solid fa-coins', typeClass: 'extra-income' },
        {
          title: 'Conta a Pagar',
          value: 3300.0,
          iconClass: 'fa-solid fa-hand-holding-dollar',
          typeClass: 'bills-to-pay',
        },
        {
          title: 'Déficit',
          value: 177.0,
          iconClass: 'fa-solid fa-credit-card',
          typeClass: 'deficit',
        },
        {
          title: 'Poupança',
          value: 450.0,
          iconClass: 'fa-solid fa-sack-dollar',
          typeClass: 'savings',
        },
      ],
      chartCategories: [
        'Jan',
        'Fev',
        'Mar',
        'Abr',
        'Mai',
        'Jun',
        'Jul',
        'Ago',
        'Set',
        'Out',
        'Nov',
        'Dez',
      ],
      chartSeries: [
        {
          type: 'line',
          name: 'Saldo',
          color: '#0097FF',
          data: [4000, 3478, 3613, 3895, 3741, 3569, 3811, 0, 0, 0, 0, 0],
        },
        {
          type: 'line',
          name: 'Dívidas',
          color: '#E74C3C',
          data: [3500, 3651, 2876, 4121, 3412, 3269, 3988, 0, 0, 0, 0, 0],
        },
      ],
    };

    return of(mockData);
  }
}
