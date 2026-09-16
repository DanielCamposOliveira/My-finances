import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';

import { TableContafixa } from '../../components/Tabela/contafixa/TableContafixa.component';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { ContaFixa } from '../../models/canta-fixa';

@Component({
  selector: 'app-conta.page',
  standalone: true,
  imports: [CommonModule, TableContafixa],
  templateUrl: './conta.page.html',
  styleUrl: './conta.page.scss',
})
export class ContaPage implements OnInit {

  private contaFixaService = inject(ContaFixaService);
  
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);

  ContaFixas: ContaFixa[] = [];

  ngOnInit(): void {
    this.obterContasFixas();
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


  // Navegação para o cadastro de Lançamento
  onPageCadastroLancamento(): void {
    this.router.navigate(['/cadastro-lancamento']);
  }

  // Navegação para o cadastro de Conta Fixa
  onPageCadastroContaFixa(): void {
    this.router.navigate(['/cadastro-contafixa']);
  }
}
