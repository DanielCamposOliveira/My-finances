import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { combineLatest } from 'rxjs';

import { MoneyCard } from '../../components/card/money-card/money-card';
import { MoneyChart } from '../../components/graphic/money-chart/money-chart';
import { DashboardServe } from '../../service/Dashboard/dashboard-serve';
import { MoneyTable } from '../../components/Tabela/money-table/money-table';
import { ContaPendenteItemModel, HistoricoFinanceiroAnualModel, DashboardCardItemModel } from '../../models/InterfaceModel';
//import {TagService} from  '../../service/Tag/tag-service'
//import {CategoriaService} from '../../service/Categoria/categoria-service'
//import { Tag } from '../../models/tag.model';
//import { Categoria } from '../../models/categoria.model';

import {FinanceiroService} from '../../service/Financeiro/financeiro-service'
import { LocalstorageService } from '../../service/localstorage/localstorage-service'

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MoneyCard, MoneyChart, MoneyTable],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {

  private financeiroService = inject(FinanceiroService);
  private localStorageService = inject(LocalstorageService);

  private dashboardService = inject(DashboardServe);
  private cdr = inject(ChangeDetectorRef); // serve para forçar o Angular a atualizar a tela (HTML)

  historicoFinanceiro?: HistoricoFinanceiroAnualModel;

  // Cards do Dashboard
  cardDividaPendente?: DashboardCardItemModel;
  cardValorReceber?: DashboardCardItemModel;
  cardValorSaldo?: DashboardCardItemModel;
  cardDeficit?: DashboardCardItemModel;

  contasPendentes: ContaPendenteItemModel[] = [];

  //tags?: Tag;
  //private tagService = inject(TagService);
  //TagNome = "Tag_Angula";



  //Categorias?: Categoria;
  //private categoriaService = inject(CategoriaService);


  
  ngOnInit(): void {

    this.ObterSaldo();
    this.ObterValorReceber();
    this.ObterValorDividaPendente();
    this.ObterValorDeficitReativo();

    this.ObterGrafico();

    this.ObterListaContaPendente();

    this.verificarEGerarHistorico();
  }

  ObterGrafico(): void {
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
  }

  ObterSaldo(): void {
    this.dashboardService.getValorSaldo().subscribe({
      next: (resposta: any) => {
           
        // Atualiza o objeto do card
        this.cardValorSaldo = {
          title: 'Saldo',
          value: resposta,
          iconClass: 'fa-solid fa-wallet',
          typeClass: 'salary',
        };

        this.cdr.markForCheck(); // Marca para atualização visual        
      },
      error: (err) => {
        console.error('Erro ao carregar valor em saldo:', err);
      }
    });
  }

  ObterValorReceber(): void {
    this.dashboardService.getValorReceber().subscribe({
      next: (resposta: any) => {
 
        // Atualiza o objeto do card
        this.cardValorReceber = {
          title: 'Receber',
          value: resposta,
          iconClass: 'fa-solid fa-coins',
          typeClass: 'extra-income',
        };
        
        this.cdr.markForCheck(); // Marca para atualização visual        
      },
      error: (err) => {
        console.error('Erro ao carregar valor a receber:', err);
      }
    });
  }

  ObterValorDividaPendente(): void {
    this.dashboardService.getDividaPendente().subscribe({
      next: (resposta: any) => {
       
        // Atualiza o objeto do card
        this.cardDividaPendente = {
          title: 'Conta a Pagar',
          value: resposta,
          iconClass: 'fa-solid fa-hand-holding-dollar',
          typeClass: 'bills-to-pay',
        };
        this.cdr.markForCheck(); // Marca para atualização visual        
      },
      error: (err) => {
        console.error('Erro ao carregar valor a receber:', err);
      }
    });
  }

  ObterValorDeficitReativo(): void {
    combineLatest([
      this.dashboardService.getValorSaldo(),
      this.dashboardService.getDividaTotal()
    ]).subscribe({
      next: ([saldo, divida]) => {
        const valorSaldo = saldo ?? 0;
        const valorDivida = divida ?? 0;

        const resultado = valorSaldo - valorDivida;

        if (resultado < 0) {
          this.cardDeficit = {
            title: 'Déficit',
            value: Math.abs(resultado),
            iconClass: 'fa-solid fa-credit-card',
            typeClass: 'deficit',
          };
        } else if (resultado > 0) {
          this.cardDeficit = {
            title: 'Saldo restante',
            value: resultado,
            iconClass: 'fa-solid fa-wallet',
            typeClass: 'saldo',
          };
        } else {
          this.cardDeficit = {
            title: 'Saldo zerado',
            value: 0,
            iconClass: 'fa-solid fa-equals',
            typeClass: 'neutro',
          };
        }

        this.cdr.markForCheck();
      }
    });
  }

  ObterListaContaPendente(): void {
    this.dashboardService.getContasPendentesUnificadas().subscribe({
      next: (dados) => {
        //console.log('atribuicao:', dados); // Verifica os dados recebidos
        this.contasPendentes = dados;
        this.cdr.markForCheck(); // Marca para atualização visual   
      },
      error: (err) => {
        console.error('Erro ao carregar contas pendentes:', err);
      },
    });
  }


  private verificarEGerarHistorico(): void {
    // Retorna true se já foi gerado neste mês, false se precisa gerar
    // Verfiica no LocalStorage se ja foi gerado o grafico do mes passado, caso não não esteja gerado chama a API para gera
    const jaGeradoNoMes = this.localStorageService.historyMonthly();

    if (!jaGeradoNoMes) {
      this.financeiroService.GetCheckHistory().subscribe({
        next: (res) => {
          if (res.status === 201) {
            this.cdr.detectChanges();
            console.log('Histórico gerado com sucesso:', res.message);
          } else if (res.status === 409) {
            console.warn('Registro já existia no banco:', res.message);
          } else {
            console.error('Falha ao gerar histórico:', res.message);
          }
        },
        error: (err) => {
          console.error('Erro de conexão:', err);
        }
      });
    }
  }










  //Evento disparado pelo clique do botão
  onAtualizarClique(): void {
    this.ObterSaldo();
  }

}
