import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, startWith } from 'rxjs';
import { ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel } from '../../models/canta-fixa';
import { returnParcela } from '../../models/parcela-model';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ContaFixaService {
  private http = inject(HttpClient);
  private readonly EndPoint_ContaFixa = `${environment.apiUrl}/ContasFixas`;

  Status(Dados: ContaFixaStatusParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_ContaFixa}/parcela/update/status`;
    return this.http.patch<void>(Url, Dados);
  }

  Valor(Dados: ContaFixaValorParcelaModel): Observable<void> {
    const Url = `${this.EndPoint_ContaFixa}/parcela/update/valor`;
    return this.http.patch<void>(Url, Dados);
  }

  Parcelas(): Observable<returnParcela[]> {
    const url = `${this.EndPoint_ContaFixa}/parcela/pendentes`;
    return this.http.get<returnParcela[]>(url).pipe(
      startWith([] as returnParcela[])
    );
  }
}