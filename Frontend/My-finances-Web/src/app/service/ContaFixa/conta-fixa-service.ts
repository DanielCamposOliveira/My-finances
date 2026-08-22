import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContaFixaCadastroModel, ContaFixaStatusModel, ContaFixaStatusParcelaModel, ContaFixaValorParcelaModel } from '../../models/canta-fixa';

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

    Patch_ParcelaStatus(Dados: ContaFixaStatusParcelaModel): Observable<void> {
        const Url = `${this.EndPoint_ContaFixa}/parcela/update/status`
        return this.http.patch<void>(Url, Dados);
    }

    Patch_ParcelaValor(Dados: ContaFixaValorParcelaModel): Observable<void> {
        const Url = `${this.EndPoint_ContaFixa}/parcela/update/valor`;
        return this.http.patch<void>(Url, Dados);
    }

}
