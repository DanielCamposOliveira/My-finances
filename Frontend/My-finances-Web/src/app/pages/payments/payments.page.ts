import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { TablePayments } from '../../components/Tabela/payments/tablePayments';

import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { UploadService } from '../../service/Upload/upload-service';

import { returnParcela } from '../../models/parcela.model';
import { LancamentoStatusParcelaModel, LancamentoValorParcelaModel, LancamentoProofPaymentParcelaModel } from '../../models/lancamentos.model';
import { ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel, ContaFixaProofPaymentParcelaModel } from '../../models/contaFixa.model';


//import { Router } from '@angular/router';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, TablePayments],
  templateUrl: './payments.page.html',
  styleUrl: './payments.page.scss',
})
export class PaymentsPage implements OnInit {

  private route = inject(ActivatedRoute);
  private contaFixaService = inject(ContaFixaService);
  private lancamentosService = inject(LancamentosService);
  private cdr = inject(ChangeDetectorRef);

  // Injeção de dependência do serviço de upload
  constructor(private uploadService: UploadService) { }


  entityId: string = '';
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






  UploadComprovante(dados: { file: File; parcela: returnParcela }, origem: string): void {
    const { file, parcela } = dados;
    if (!file) return;

    this.uploadService.obterUrlUpload().subscribe({
      next: async (permissao) => {
        try {
          const resultado = await this.uploadService.uploadParalelo(file, permissao.url, () => { });

          if (resultado.sucesso && resultado.token) {
            if (origem === 'conta-fixa') {
              this.atualizarComprovanteContaFixa(parcela.id, resultado.token);
            } else {
              this.atualizarComprovanteLancamento(parcela.id, resultado.token);
            }
          }
        } catch (error) {
          console.error('Ocorreu um erro ao tentar fazer o upload do comprovante:', error);
        }
      },
      error: (err) => console.error('Erro ao obter URL:', err)
    });
  }

  private atualizarComprovanteContaFixa(parcelaId: number, token: string): void {
    const payload: ContaFixaProofPaymentParcelaModel = {
      parcelaId: parcelaId,
      proofPayment: token
    };
    this.contaFixaService.UpdateProofPayment(payload).subscribe({
      next: () => {
        // Recarrega usando o ID da rota original
        this.carregarDados(this.tipoOrigem, this.entityId);
      },
      error: (err) => console.error('Erro ao salvar Comprovante de Pagamento (Conta Fixa):', err)
    });
  }

  private atualizarComprovanteLancamento(parcelaId: number, token: string): void {
    const payload: LancamentoProofPaymentParcelaModel = {
      parcelaId: parcelaId,
      proofPayment: token
    };
    this.lancamentosService.UpdateProofPayment(payload).subscribe({
      next: () => {
        // Recarrega usando o ID da rota original
        this.carregarDados(this.tipoOrigem, this.entityId);
      },
      error: (err) => console.error('Erro ao salvar Comprovante de Pagamento (Lançamento):', err)
    });
  }


  // Recebe o objeto emitido pelo componente filho
  DownloadComprovante(evento: { parcela: returnParcela; Tipo: string }): void {
    const { parcela, Tipo } = evento;
   
    const service$ = Tipo === 'conta-fixa'
      ? this.contaFixaService.ObterProofPayment(parcela.id)
      : this.lancamentosService.ObterProofPayment(parcela.id);

    service$.subscribe({
      next: (resposta: { url: string } | any) => {
        const link = typeof resposta === 'string' ? resposta : resposta?.url;
        if (link) {
          window.open(link, '_blank');
        } else {
          console.warn('URL do comprovante não encontrada na resposta.');
        }
      },
      error: (err) => console.error('Erro ao obter comprovante:', err)
    });
  }













}
