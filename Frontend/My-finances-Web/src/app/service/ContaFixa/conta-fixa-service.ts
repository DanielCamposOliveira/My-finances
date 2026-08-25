import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContaFixaCadastroModel, ContaFixaStatusModel, ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel, returnParcelaModel } from '../../models/canta-fixa';

@Injectable({
    providedIn: 'root',
})

export class ContaFixaService {
    private http = inject(HttpClient);
    private readonly EndPoint_ContaFixa = "http://192.168.0.5:5000/api/v1/ContasFixas";

    Post_ContaFixa(Dados: ContaFixaCadastroModel): Observable<void> {
        return this.http.post<void>(this.EndPoint_ContaFixa, Dados);
    }

    Patch_ContaFixa(Dados: ContaFixaStatusModel): Observable<void> {
        const Url = `${this.EndPoint_ContaFixa}/update/status`
        return this.http.patch<void>(Url, Dados);
    }


    //Atualiza o status
    Status(Dados: ContaFixaStatusParcelaModel): Observable<void> {
        const Url = `${this.EndPoint_ContaFixa}/parcela/update/status`
        return this.http.patch<void>(Url, Dados);
    }

    //Atualiza o valor
    Valor(Dados: ContaFixaValorParcelaModel): Observable<void> {
        const Url = `${this.EndPoint_ContaFixa}/parcela/update/valor`;
        return this.http.patch<void>(Url, Dados);
    }

    // Obtem todas as parcelas pendentes
    Parcelas(): Observable<returnParcelaModel[]>{
        const url = `${this.EndPoint_ContaFixa}/parcela/pendentes`;
        return this.http.get<returnParcelaModel[]>(url);
    }


}
