import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, startWith } from 'rxjs';
import { Categoria,CategoriaCadastro } from '../../models/categoria.model';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
    
export class CategoriaService {
     
  private http = inject(HttpClient);
  private readonly Url = `${environment.apiUrl}/categorias`;

  getCategoria(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(this.Url).pipe(startWith([] as Categoria[]));
  };
  
  PostCategoria(Dados: CategoriaCadastro): Observable<void> {
    return this.http.post<void>(this.Url, Dados);
  }

}