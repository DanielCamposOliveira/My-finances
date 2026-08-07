import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CategoriaModel } from '../../models/InterfaceModel';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
    
export class CategoriaService {
     private http = inject(HttpClient);
    private readonly Url = "http://localhost:5000/api/v1/categorias";

    getCategoria(): Observable<CategoriaModel[]>{
        return this.http.get<CategoriaModel[]>(this.Url);
    };
}
