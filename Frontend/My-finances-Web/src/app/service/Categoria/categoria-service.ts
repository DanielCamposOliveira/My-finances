import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoriaModel,CategoriaCadastroModel } from '../../models/categoria';


@Injectable({
  providedIn: 'root',
})
    
export class CategoriaService {
     
  private http = inject(HttpClient);
  private readonly Url = "http://192.168.0.5:5000/api/v1/categorias";

  getCategoria(): Observable<CategoriaModel[]> {
    return this.http.get<CategoriaModel[]>(this.Url);
  };
  
  PostCategoria(Dados: CategoriaCadastroModel): Observable<void> {
    return this.http.post<void>(this.Url, Dados);
  }

}
