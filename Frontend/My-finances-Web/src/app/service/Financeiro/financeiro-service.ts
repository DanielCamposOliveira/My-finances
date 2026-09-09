import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';

import { environment } from '../../../environments/environment';

import {ResponseGenerator} from '../../models/InterfaceModel'


@Injectable({
    providedIn: 'root',
})
    
export class FinanceiroService {

    private http = inject(HttpClient);
    private readonly EndPoint_Generator = `${environment.apiUrl}`;
  

    GetCheckHistory(): Observable<ResponseGenerator> {
        const Url = `${this.EndPoint_Generator}/HistoricoFinanceiroAnual/generator`
        return this.http.post(Url, {}, {
            observe: 'response',
            responseType: 'text' // Garante leitura caso a API retorne texto puro ou corpo vazio no 201
        }).pipe(
            map((response: HttpResponse<string>) => ({
                status: response.status,
                message: response.body || 'Histórico gerado com sucesso!'
            })),
            catchError((error: HttpErrorResponse) => {
                let errorMsg = 'Erro inesperado ao processar o histórico.';

                if (typeof error.error === 'string') {
                    errorMsg = error.error;
                } else if (error.error?.detail) {
                    // Captura mensagens padrão do Results.Problem() do ASP.NET Core
                    errorMsg = error.error.detail;
                }

                return of({
                    status: error.status,
                    message: errorMsg
                });
            })
        );
    }    
}
