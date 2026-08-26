import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableParcela } from '../../components/Tabela/contas/TableParcela';

import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';

import { LancamentoStatusParcelaModel } from '../../models/lancamentos';
import { ContaFixaStatusParcelaModel } from '../../models/canta-fixa';

import { StatusParcelaEnum } from '../../enums/status-parcela-enum';
import { returnParcela } from '../../models/parcela-model';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [CommonModule, TableParcela],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.scss',
})
export class Cadastro implements OnInit {
  private lancamentosService = inject(LancamentosService);
  private contaFixaService = inject(ContaFixaService);
  private cdr = inject(ChangeDetectorRef);

  parcelasLancamentos: returnParcela[] = [];
  parcelasContaFixa: returnParcela[] = [];

  ngOnInit(): void {
    this.obterLancamentos();
    this.obterContasFixas();
  }

  obterLancamentos(): void {
    this.lancamentosService.Parcelas().subscribe({
      next: (resposta) => {        
        this.parcelasLancamentos = [...resposta];
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Erro ao carregar parcelas de lançamentos:', err)
    });
  }

  obterContasFixas(): void {
    this.contaFixaService.Parcelas().subscribe({
      next: (resposta) => {
        this.parcelasContaFixa = [...resposta];
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Erro ao carregar parcelas de contas fixas:', err)
    });
  }

  pagarLancamento(parcela: returnParcela): void {
    const payload: LancamentoStatusParcelaModel = {
      parcelaId: parcela.id,
      status: StatusParcelaEnum.Pago
    };

    this.lancamentosService.Status(payload).subscribe({
      next: () => {
        this.obterLancamentos();
      },
      error: (err) => console.error('Erro ao pagar parcela do lançamento:', err)
    });
  }

  pagarContaFixa(parcela: returnParcela): void {
    const payload: ContaFixaStatusParcelaModel = {
      parcelaId: parcela.id,
      status: StatusParcelaEnum.Pago
    };

    this.contaFixaService.Status(payload).subscribe({
      next: () => {
        this.obterContasFixas();
      },
      error: (err) => console.error('Erro ao pagar parcela de conta fixa:', err)
    });
  }
}