import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, startWith } from 'rxjs';
import { LancamentoStatusParcelaModel, LancamentoCadastro, LancamentoValorParcelaModel, Lancamento, LancamentoResponseList, LancamentoProofPaymentParcelaModel } from '../../models/lancamentos.model';
import { returnParcela } from '../../models/parcela.model';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
    
export class LancamentosService {
  private http = inject(HttpClient);
  private readonly EndPoint_Lancamento = `${environment.apiUrl}/lancamentos`;

  //Atualiza o status
  UpdateStatus(Dados: LancamentoStatusParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_Lancamento}/parcela/update/status`;
    return this.http.patch<void>(Url, Dados);
  }
   
  //Atualiza o valor da parcela conta fixa
  UpdateValor(Dados: LancamentoValorParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_Lancamento}/parcela/update/valor`;
    return this.http.patch<void>(Url, Dados);
  }

  //Atualiza ProofPayment da parcela Lancamento
  UpdateProofPayment(Dados: LancamentoProofPaymentParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_Lancamento}/parcela/update/ProofPayment`;
    return this.http.patch<void>(Url, Dados);
  }
    
  // Obtem todas as parcelas
  ListParcelas(): Observable<returnParcela[]> {
    const url = `${this.EndPoint_Lancamento}/parcela`;
    return this.http.get<returnParcela[]>(url).pipe(
      startWith([] as returnParcela[])
    );
  }

  // ----------------- Lista todas as parcelas de todos os Lancamentos
  ListParcelasPagasPorId(id: number | string): Observable<returnParcela[]> {
    const url = `${this.EndPoint_Lancamento}/parcela/ProofPayment/${id}`;
    return this.http.get<returnParcela[]>(url).pipe(
      startWith([] as returnParcela[])
    );
  }
    
  //Lista todas as Contas Fixa
  ListLancamentos(): Observable<LancamentoResponseList[]> {
    const url = `${this.EndPoint_Lancamento}`;
    
    return this.http.get<LancamentoResponseList[]>(url).pipe(
      startWith([] as LancamentoResponseList[])
    );
  }

  // cadastra lancamentos
  CreateLancamento(Dados: LancamentoCadastro): Observable<void> {
    console.log("Dados sendo enviados: ", Dados);
    const url = `${this.EndPoint_Lancamento}`;
    return this.http.post<void>(url, Dados);
  }


}
