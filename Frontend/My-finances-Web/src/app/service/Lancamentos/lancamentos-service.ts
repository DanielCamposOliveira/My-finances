import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, startWith } from 'rxjs';
import { LancamentoStatusParcelaModel, LancamentoCadastro } from '../../models/lancamentos.model';
import { returnParcela } from '../../models/parcela-model';

import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root',
})
    
export class LancamentosService {
    private http = inject(HttpClient);
    private readonly EndPoint_Lancamento = `${environment.apiUrl}/lancamentos`;

    //Atualiza o status
    Status(Dados: LancamentoStatusParcelaModel): Observable<void> {
        const Url = `${this.EndPoint_Lancamento}/parcela/update/status`;
        return this.http.patch<void>(Url, Dados);
    }
   
    // Obtem todas as parcelas
    Parcelas(): Observable<returnParcela[]> {
        const url = `${this.EndPoint_Lancamento}/parcela`;
        return this.http.get<returnParcela[]>(url).pipe(
            startWith([] as returnParcela[])
        );
    }

    // cadastra lancamentos
    Lancamento(Dados: LancamentoCadastro): Observable<void>{
        console.log("Dados sendo enviados: ", Dados);
        const url = `${this.EndPoint_Lancamento}`;
        return this.http.post<void>(url, Dados);
    }
}
