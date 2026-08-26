import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoriaModel,CategoriaCadastroModel } from '../../models/categoria';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
    
export class CategoriaService {
     
  private http = inject(HttpClient);
  private readonly Url = `${environment.apiUrl}/categorias`;

  getCategoria(): Observable<CategoriaModel[]> {
    return this.http.get<CategoriaModel[]>(this.Url);
  };
  
  PostCategoria(Dados: CategoriaCadastroModel): Observable<void> {
    return this.http.post<void>(this.Url, Dados);
  }

}
