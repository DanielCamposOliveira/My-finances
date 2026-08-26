import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, startWith } from 'rxjs';
import { LancamentoStatusParcelaModel } from '../../models/lancamentos';
import { returnParcela } from '../../models/parcela-model';

@Injectable({
    providedIn: 'root',
})
    
export class LancamentosService {
    private http = inject(HttpClient);
    private readonly EndPoint_Lancamento = "http://192.168.0.5:5000/api/v1/lancamentos";

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
}
