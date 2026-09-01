import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { switchMap } from 'rxjs/operators';
import { Observable, startWith } from 'rxjs';
import { ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel, ContaFixaCadastro } from '../../models/canta-fixa';
import { returnParcela } from '../../models/parcela-model';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ContaFixaService {
  private http = inject(HttpClient);
  private readonly EndPoint_ContaFixa = `${environment.apiUrl}/ContasFixas`;

  //Cria Conta Fixa e chama o endpoint de gera a pacela do mes atual
  ContaFixa(Dados: ContaFixaCadastro): Observable<void> {
    const url = `${this.EndPoint_ContaFixa}`;
    return this.http.post<void>(url, Dados).pipe(
      switchMap(() => {
        const Url = `${this.EndPoint_ContaFixa}/generator`;
        return this.http.post<void>(Url, Dados);
      })
    );
  }

  //Atualiza o status da parcela conta fixa
  Status(Dados: ContaFixaStatusParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_ContaFixa}/parcela/update/status`;
    return this.http.patch<void>(Url, Dados);
  }

  //Atualiza o valor da parcela conta fixa
  Valor(Dados: ContaFixaValorParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_ContaFixa}/parcela/update/valor`;
    return this.http.patch<void>(Url, Dados);
  }

  //Lista todas as parcelas de todos os Lancamentos
  Parcelas(): Observable<returnParcela[]> {
    const url = `${this.EndPoint_ContaFixa}/parcela/pendentes`;
    return this.http.get<returnParcela[]>(url).pipe(
      startWith([] as returnParcela[])
    );
  }
}