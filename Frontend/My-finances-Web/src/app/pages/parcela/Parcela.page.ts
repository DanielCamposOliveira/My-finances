import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';


import { TableParcela } from '../../components/Tabela/Parcela/TableParcela';

import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { UploadService } from '../../service/Upload/upload-service';

import { LancamentoStatusParcelaModel, LancamentoValorParcelaModel, LancamentoProofPaymentParcelaModel } from '../../models/lancamentos.model';
import { ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel, ContaFixaProofPaymentParcelaModel } from '../../models/contaFixa.model';
import { StatusParcelaEnum } from '../../enums/status-parcela-enum';
import { returnParcela } from '../../models/parcela.model';

 
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

  // Injeção de dependência do serviço de upload
  constructor(private uploadService: UploadService) { }

  private cdr = inject(ChangeDetectorRef);

  
  parcelasLancamentos: returnParcela[] = [];
  parcelasContaFixa: returnParcela[] = [];

  
  ngOnInit(): void {
    this.obterLancamentos();
    this.obterContasFixas();
  }

  obterLancamentos(): void {
    this.lancamentosService.ListParcelas().subscribe({
      next: (resposta) => {
        this.parcelasLancamentos = [...resposta];
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Erro ao carregar parcelas de lançamentos:', err)
    });
  }

  obterContasFixas(): void {
    this.contaFixaService.ListParcelas().subscribe({
      next: (resposta) => {
        this.parcelasContaFixa = [...resposta];
        this.cdr.markForCheck();
      },
      error: (err) => console.error('Erro ao carregar parcelas de contas fixas:', err)
    });
  }



  // Atualiza o valor\status 
  pagarLancamento(parcela: returnParcela): void {
    const payload: LancamentoStatusParcelaModel = {
      parcelaId: parcela.id,
      status: StatusParcelaEnum.Pago
    };

    this.lancamentosService.UpdateStatus(payload).subscribe({
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

    this.contaFixaService.UpdateStatus(payload).subscribe({
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
    this.contaFixaService.UpdateValor(payload).subscribe({
      next: () => {
        this.obterContasFixas();
      },
      error: (err) => console.error('Erro ao altera o valor da parcela de conta fixa:', err)
    });
  }

  ValorLancamento(parcela: LancamentoValorParcelaModel): void {
    
    const payload: LancamentoValorParcelaModel = {
      parcelaId: parcela.parcelaId,
      valorParcela: parcela.valorParcela
    };
    this.lancamentosService.UpdateValor(payload).subscribe({
      next: () => {
        this.obterLancamentos();
      },
      error: (err) => console.error('Erro ao altera o valor da parcela de conta fixa:', err)
    });
  }

 

  UploadComprovantePagamento( dados: { file: File; parcela: returnParcela }, origem: 'conta-fixa' | 'lancamento' ): void {
    const { file, parcela } = dados;

    if (!file) return;

    // 1. Solicita a permissão/URL de upload para a API
    this.uploadService.obterUrlUpload().subscribe({
      next: async (permissao) => {
        try {
          // 2. Envia o arquivo em paralelo passando o arquivo, URL e a callback de progresso
          const resultado = await this.uploadService.uploadParalelo( file, permissao.url, (progresso) => {
              //console.log(`Progresso: ${progresso.percentual}%`);
            }
          );
       
          // Trata o desfecho positivo reportado pelo serviço
          if (resultado.sucesso && resultado.token) {
            // Roteamento de acordo com a origem
            if (origem === 'conta-fixa') {
              this.atualizarComprovanteContaFixa(parcela.id, resultado.token);
            } else {
              this.atualizarComprovanteLancamento(parcela.id, resultado.token);
            }
          }
        } catch (error) {
          console.error('Erro no upload paralelo:', error);
        }
      },
      error: (err) => console.error('Erro ao obter URL:', err)
    });
  }



  // Métodos auxiliares para cada serviço:
  private atualizarComprovanteContaFixa(parcelaId: number, token: string): void {
    
    const payload: ContaFixaProofPaymentParcelaModel ={
      parcelaId: parcelaId,
      proofPayment : token      
    }
    this.contaFixaService.UpdateProofPayment(payload).subscribe({
      next: () => {
        this.obterContasFixas();
      },
      error: (err) => {
        console.error('Ocorreu um erro ao tentar salvar o Comprovante de Pagamento:', err)
      }
    });
    
  }

  private atualizarComprovanteLancamento(parcelaId: number, token: string): void {
   
    const payload: LancamentoProofPaymentParcelaModel ={
      parcelaId: parcelaId,
      proofPayment : token      
    }

    
    this.lancamentosService.UpdateProofPayment(payload).subscribe({
      next: () => {
        this.obterLancamentos();
      },
      error: (err) => {
        console.error('Ocorreu um erro ao tentar salvar o Comprovante de Pagamento:', err)
      }
    });
  }






}