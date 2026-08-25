import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LancamentosModel, LancamentoParcelaModel, returnParcelaModel } from '../../models/lancamentos';

@Injectable({
    providedIn: 'root',
})
    
export class LancamentosService {
    private http = inject(HttpClient);
    private readonly EndPoint_Lancamento = "http://192.168.0.5:5000/api/v1/lancamentos";

    Post_Lancamento(Dados: LancamentosModel): Observable<void> {
        return this.http.post<void>(this.EndPoint_Lancamento, Dados);
    }

    //Atualiza o status
    Status(Dados: LancamentoParcelaModel): Observable<void> {
        const Url = `${this.EndPoint_Lancamento}/parcela/update/status`;
        return this.http.patch<void>(Url, Dados);
    }


    
    // Obtem todas as parcelas
    Parcelas(): Observable<returnParcelaModel[]> {
        const url = `${this.EndPoint_Lancamento}/parcela`;
        return this.http.get<returnParcelaModel[]>(url);
    }

    // Obtem todas as parcelas pendentes
    ParcelasPendentes(): Observable<returnParcelaModel[]> {
        const url = `${this.EndPoint_Lancamento}/parcela/pendentes`;
        return this.http.get<returnParcelaModel[]>(url);
    }

}
