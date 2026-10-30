import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { TablePayments } from '../../components/Tabela/payments/tablePayments';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { returnParcela } from '../../models/parcela.model';

//import { Router } from '@angular/router';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, TablePayments],
  templateUrl: './payments.page.html',
  styleUrl: './payments.page.scss',
})
export class PaymentsPage implements OnInit{

  private route = inject(ActivatedRoute);
  private contaFixaService = inject(ContaFixaService);
  private lancamentosService = inject(LancamentosService);
  private cdr = inject(ChangeDetectorRef);

  entityId: string | number = '';
  tipoOrigem: string = '';
  parcelasPagas: returnParcela[] = [];

  ngOnInit(): void {
    // Obtém o parâmetro 'id' da rota atual
    const id = this.route.snapshot.paramMap.get('id');
    const tipo = this.route.snapshot.paramMap.get('tipo');

   if (id && tipo) {
      this.entityId = id;
      this.tipoOrigem = tipo;
      this.carregarDados(tipo, id);
    }
  }



  carregarDados(tipo: string, id: string): void {
    const obs$ = tipo === 'conta-fixa'
      ? this.contaFixaService.ListParcelasPagasPorId(id)
      : this.lancamentosService.ListParcelasPagasPorId(id); // método correspondente no LancamentosService

    obs$.subscribe({
      next: (resposta) => {       
        this.parcelasPagas = [...resposta];
        this.cdr.markForCheck();
      },
      error: (err) => console.error(`Erro ao carregar parcelas pagas (${tipo}):`, err)
    });
  }

}
