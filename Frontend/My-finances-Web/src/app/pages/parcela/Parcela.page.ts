import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';


import { Router } from '@angular/router';

import { TableParcela } from '../../components/Tabela/contas/TableParcela';

import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { TagService } from '../../service/Tag/tag-service';
import { CategoriaService } from '../../service/Categoria/categoria-service';

import { LancamentoStatusParcelaModel, LancamentoCadastro } from '../../models/lancamentos.model';
import { ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel } from '../../models/canta-fixa';

import { StatusParcelaEnum } from '../../enums/status-parcela-enum';
import { returnParcela } from '../../models/parcela-model';

import { Tag } from '../../models/tag.model';
import { Categoria } from '../../models/categoria.model';

import { erase, error } from 'highcharts';
 
@Component({
  selector: 'app-parcela',
  standalone: true,
  imports: [CommonModule, TableParcela],
  templateUrl: './Parcela.page.html',
  styleUrl: './Parcela.page.scss',
})
export class parcelaPage implements OnInit {

  private lancamentosService = inject(LancamentosService);
  private contaFixaService = inject(ContaFixaService);
  private categoriaService = inject(CategoriaService);
  private tagService = inject(TagService);


  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  
  parcelasLancamentos: returnParcela[] = [];
  parcelasContaFixa: returnParcela[] = [];

  tag: Tag[] = [];
  categoria: Categoria[] = [];

  
  ngOnInit(): void {
    this.obterLancamentos();
    this.obterContasFixas();
    this.obterTag();
    this.obterCategoria();
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

  obterTag(): void {
    this.tagService.GetTag().subscribe({
      next: (resposta) => {
        this.tag = [...resposta];
      },
      error: (erro) => console.log("Tag Erro:", erro)
    });
  }

  obterCategoria(): void {
    this.categoriaService.getCategoria().subscribe({
      next: (resposta) => {
        this.categoria = [...resposta];
      },
      error: (Error) => console.log("Categoria Erro:", error)
    });
  }


  // Atualiza o valor\status 
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

  ValorContaFixa(parcela: ContaFixaValorParcelaModel): void {
    
    const payload: ContaFixaValorParcelaModel = {
      parcelaId: parcela.parcelaId,
      valorParcela: parcela.valorParcela
    };
    this.contaFixaService.Valor(payload).subscribe({
      next: () => {
        this.obterContasFixas();
      },
      error: (err) => console.error('Erro ao altera o valor da parcela de conta fixa:', err)
    });
  }


  //navegarParaCadastroComDados(parcelaId: number): void {
  //this.router.navigate(['/cadastro'], { queryParams: { id: parcelaId } });
  //}
  
  onPageCadastro(): void {   
    this.router.navigate(['/cadastro']);
  }


}