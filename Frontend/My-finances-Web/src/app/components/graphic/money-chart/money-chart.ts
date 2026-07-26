import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'app-money-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './money-chart.html',
  styleUrl: './money-chart.scss',
})
export class MoneyChart implements OnInit {

  ngOnInit(): void {
    setTimeout(() => {
      Highcharts.chart('chart-container', {
        chart: {
          plotBorderColor: 'var(--highcharts-neutral-color-10, #e6e6e6)',
          plotBorderWidth: 1,
          plotBorderRadius: 5
        },

        title: {
          text: 'Evolução Financeira',
          align: 'left'
        },

        subtitle: {
          text: 'Saldo x Dívidas'
        },

        yAxis: {
          title: {
            text: 'Valor (R$)'
          }
        },

        xAxis: {
          categories: [
            'Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
            'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'
          ],
          crosshair: true,
          lineWidth: 0,
          tickLength: 6,
          tickColor: 'var(--highcharts-neutral-color-10, #e6e6e6)'
        },

        legend: {
          enabled: true
        },

        plotOptions: {
          series: {
            marker: {
              enabled: true
            },
            dataLabels: {
              enabled: true,
                format: 'R$ {y:,.2f}', // Formata como moeda (R$ 3.478,00)
                style: {
                  fontSize: '11px',
                  fontWeight: 'bold'
                }
            }
          }
        },

        tooltip: {
          shared: true,
          formatter: function (this: any) {
            const points = this.points || [];
            const saldo = points.find((p: any) => p.series.name === 'Saldo')?.y || 0;
            const dividas = points.find((p: any) => p.series.name === 'Dívidas')?.y || 0;
            const restante = saldo - dividas;

            const titulo = restante < 0 ? 'Déficit' : 'Crédito';
            const cor = restante < 0 ? '#e74c3c' : '#2ecc71';

            return `
              <b></b><br>
              <hr style="margin: 4px 0;">
          <b>${titulo}: <span style="color:${cor}">
          R$ ${Highcharts.numberFormat(Math.abs(restante), 2, ',', '.')}
          </span></b>
            `;
          }
        },

        series: [{
          type: 'line',
          name: 'Saldo',
          color: '#0097FF',
          data: [
            3525, 3478, 3613, 3895,
            3741, 3569, 3811, 3697,
            3924, 3777, 3643, 3858
          ]
        }, {
          type: 'line',
          name: 'Dívidas',
          color: '#E74C3C',
         
          data: [
            2980, 3651, 2876, 4121,
            3412, 3269, 3988, 3525,
            3718, 4058, 3342, 4296
          ]
        }]
      });
    }, 0);
  }

}