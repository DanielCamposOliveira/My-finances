import { Component, inject, OnInit } from '@angular/core';
import { TableContasFixa } from "../../components/Tabela/contas/TableContasFixa";

import { LancamentosService } from "../../service/Lancamentos/lancamentos-service";
import { returnParcelaModel } from '../../models/lancamentos';
import { error } from 'highcharts';


@Component({
  selector: 'app-cadastro',
  imports: [TableContasFixa],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.scss',
})
export class Cadastro implements OnInit {
  private LancamentosService = inject(LancamentosService);

  lancamentoRetornoModel?: returnParcelaModel[];
  
  ngOnInit(): void {
    this.ObterLancamentos();
  }

  ObterLancamentos(): void {
    this.LancamentosService.Parcelas().subscribe({
      next: (resposta: returnParcelaModel[]) => {
        this.lancamentoRetornoModel = resposta
        console.log(this.lancamentoRetornoModel);
      },
      error: (err) => {
        console.error('Erro ao carregar valor a receber:', err);
      }
    });
  }




















}
