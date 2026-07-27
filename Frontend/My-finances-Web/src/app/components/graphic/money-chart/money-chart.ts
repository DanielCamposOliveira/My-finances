import { Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import { CommonModule } from '@angular/common';
import * as Highcharts from 'highcharts';
import {ChartSeries} from '../../../service/service.data'

@Component({
  selector: 'app-money-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './money-chart.html',
  styleUrl: './money-chart.scss',
})

export class MoneyChart implements OnChanges {

  @Input() categories: string[] = [];
  @Input() series: ChartSeries[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['categories'] || changes['series']) && this.series.length > 0) {
      this.initChart();
    }
  }

  private initChart(): void {
  setTimeout(() => {
    Highcharts.chart('chart-container', {

      // Desativa o recurso acessibilidade. ele aumenta a complecidade
      accessibility: {
        enabled: false
      },

      chart: {
        plotBorderColor: '#e6e6e6',
        plotBorderWidth: 1,
        plotBorderRadius: 5,
        style: {
          fontFamily: 'system-ui, -apple-system, sans-serif'
        }
      },

      title: {
        text: 'Histórico Financeiro',
        align: 'left'
      },



      yAxis: {
        gridLineColor: '#f0f0f0', // Linhas de grade suaves
        title: {
          text: ''
        },
        labels: {
        
        }
      },

      xAxis: {
        categories: this.categories,
        crosshair: true,
        lineWidth: 0,
        tickLength: 6,
        tickColor: '#e6e6e6',
        labels: {
          style: { color: '#495057' }
        }
      },

      legend: {
        enabled: true,
        itemStyle: { color: '#212529' }, // Cor do texto das legendas
        itemHoverStyle: { color: '#000000' }
      },

      plotOptions: {
        series: {
          marker: { enabled: true },
          dataLabels: {
            enabled: true,
            format: 'R$ {y:,.2f}',
            style: {
              fontWeight: 'bold',
              textOutline: 'none' // Remove o contorno preto em volta dos números
            }
          }
        }
      },

      tooltip: {
        shared: true,
        backgroundColor: '#ffffff', // Fundo branco do balãozinho
        borderColor: '#e0e0e0',
        borderRadius: 8,
        style: { color: '#212529' },
        formatter: function (this: any) {
          const points = this.points || [];
          const saldo = points.find((p: any) => p.series.name === 'Saldo')?.y || 0;
          const dividas = points.find((p: any) => p.series.name === 'Dívidas')?.y || 0;
          const restante = saldo - dividas;

          const titulo = restante < 0 ? 'Déficit' : 'Crédito';
          const cor = restante < 0 ? '#e74c3c' : '#2ecc71';

          return `
            <b>${this.x}</b><br>
            <hr style="margin: 4px 0; border: 0; border-top: 1px solid #eee;">
            <b>${titulo}: <span style="color:${cor}">
            R$ ${Highcharts.numberFormat(Math.abs(restante), 2, ',', '.')}
            </span></b>
          `;
        }
      },

      series: this.series.map(s => ({
        type: 'line',
        name: s.name,
        color: s.color,
        data: s.data
      }))
    });
  }, 0);
}




}