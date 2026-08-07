import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LancamentosModel, LancamentoParcelaModel } from '../../models/lancamentos';

@Injectable({
    providedIn: 'root',
})
    
export class LancamentosService {
    private http = inject(HttpClient);
    private readonly EndPoint_Lancamento = "http://localhost:5000/api/v1/lancamentos";

    Post_Lancamento(Dados: LancamentosModel): Observable<void>{
        return this.http.post<void>(this.EndPoint_Lancamento, Dados);
    }

    Patch_ParcelaStatus(Dados: LancamentoParcelaModel): Observable<void>{
        const Url = `${this.EndPoint_Lancamento}/parcela/update/status`;
        return this.http.patch<void>(Url, Dados);
    }

}
