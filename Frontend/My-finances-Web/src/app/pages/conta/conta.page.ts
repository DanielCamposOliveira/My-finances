import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';

import { TableContafixa } from '../../components/Tabela/contafixa/TableContafixa.component';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { ContaFixa } from '../../models/canta-fixa';

import {TabelaLancamentos} from '../../components/Tabela/lancamentos/TabelaLancamentos.component'
import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { LancamentoResponseList } from '../../models/lancamentos.model';

import {ContaFixaStatusModel} from '../../models/canta-fixa'

//import { TableParcela } from '../../components/Tabela/Parcela/TableParcela';

@Component({
  selector: 'app-conta.page',
  standalone: true,
  imports: [CommonModule, TableContafixa, TabelaLancamentos],
  templateUrl: './conta.page.html',
  styleUrl: './conta.page.scss',
})
export class ContaPage implements OnInit {

  private contaFixaService = inject(ContaFixaService);
  private lancamentosService = inject(LancamentosService);

  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);

  ContaFixas: ContaFixa[] = [];
  Lancamentos: LancamentoResponseList[] = [];

  ngOnInit(): void {
    this.obterContasFixas();
    this.obterLancamentos();
  }

  obterContasFixas(): void {
    this.contaFixaService.ContaFixas().subscribe({
      next: (resposta) => {
        this.ContaFixas = [...resposta];               
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Erro ao carregar parcelas de contas fixas:', err)
    });
  }

    obterLancamentos(): void {
    this.lancamentosService.Lancamentos().subscribe({
      next: (resposta) => {
        this.Lancamentos = [...resposta];  
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Erro ao carregar parcelas de contas fixas:', err)
    });
  }


    // Atualiza o valor\status 
  AtualizarStatusContaFixa(parcela: ContaFixa, ativo: boolean): void {
            
      const payload: ContaFixaStatusModel = {
        id_ContaFixa: parcela.id,
        status: ativo
      };
  
      this.contaFixaService.ContaFixasStatus(payload).subscribe({
        next: () => {
          this.obterContasFixas();
        },
        error: (err) => console.error('Erro ao pagar parcela do lançamento:', err)
      });
    }


  // Navegação para o cadastro de Lançamento
  onPageCadastroLancamento(): void {
    this.router.navigate(['/cadastro-lancamento']);
  }

  // Navegação para o cadastro de Conta Fixa
  onPageCadastroContaFixa(): void {
    this.router.navigate(['/cadastro-contafixa']);
  }
}
