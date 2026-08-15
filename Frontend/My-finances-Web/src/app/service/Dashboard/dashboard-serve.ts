import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, startWith, forkJoin, map } from 'rxjs';
import { ContaPendenteItemModel } from '../../models/InterfaceModel';
import {HistoricoFinanceiroAnualModel } from '../../models/InterfaceModel';
// Interfaces para os dados do histórico financeiro


@Injectable({
  providedIn: 'root',
})
export class DashboardServe {
  private http = inject(HttpClient);

    private readonly EndPoint =
    'http://localhost:5000/api/v1';
  
  private readonly baseUrlContasFixas =
    'http://localhost:5000/api/v1/ContasFixas/parcela/pendentes';
  
  private readonly baseUrlLancamentos =
    'http://localhost:5000/api/v1/lancamentos/parcela/pendentes';

  // Métodos para consumir as APIs
  getHistoricoFinanceiroAnual(ano: number = 2026): Observable<HistoricoFinanceiroAnualModel[]> {
    const Url = `${this.EndPoint}/HistoricoFinanceiroAnual`;
    return this.http.get<HistoricoFinanceiroAnualModel[]>(`${Url}/${ano}`);
  }

  getDividaPendente(): Observable<number> {
    const Url = `${this.EndPoint}/Consulta/Dividas/pendentes`;
  return this.http.get<number>(Url).pipe(
    startWith(0) // Emite 0 imediatamente até a API responder
  );
}

  getValorReceber(): Observable<number> {
     const Url = `${this.EndPoint}/Consulta/Valores/receber`;
  return this.http.get<number>(Url).pipe(
    startWith(0) // Emite 0 imediatamente até a API responder
  );
}

getValorSaldo(): Observable<number> {
  const Url = `${this.EndPoint}/Consulta/Valores/saldo`;
  return this.http.get<number>(Url).pipe(
    startWith(0) // Emite 0 imediatamente até a API responder
  );
}

  getContasPendentesUnificadas(): Observable<ContaPendenteItemModel[]> {
    return forkJoin({
      contasFixas: this.http.get<any[]>(this.baseUrlContasFixas),
      lancamentos: this.http.get<any[]>(this.baseUrlLancamentos),
    }).pipe(
      map(({ contasFixas, lancamentos }) => {
        // Normaliza as Contas Fixas
        const fixasMapeadas: ContaPendenteItemModel[] = contasFixas.map((item) => ({
          id: item.id,
          origemId: item.contaFixaId,
          descricao: item.descricao,
          valorParcela: item.valorParcela,
          dataVencimento: item.dataVencimento,
          dataPagamento: item.dataPagamento,
          status: item.status, // Número vindo da API
          tipo: 'CONTA_FIXA',
          atribuicao: item.atribuicao, // Número vindo da API
        }));

        // Normaliza os Lançamentos
        const lancamentosMapeados: ContaPendenteItemModel[] = lancamentos.map((item) => ({
          id: item.id,
          origemId: item.lancamento_Id,
          descricao: item.lancamento_Descricao,
          valorParcela: item.valorParcela,
          dataVencimento: item.dataVencimento,
          numeroParcela: item.numeroParcela,
          status: item.status, // Número vindo da API
          tipo: 'LANCAMENTO',
          atribuicao: item.atribuicao, // Número vindo da API
        }));

        // Une os dois arrays
        return [...fixasMapeadas, ...lancamentosMapeados];
      }),
    );
  }
}
