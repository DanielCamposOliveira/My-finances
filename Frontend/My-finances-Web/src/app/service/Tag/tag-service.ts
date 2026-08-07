import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TagModel, TagCadastroModel } from '../../models/tag';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class TagService {
    private http = inject(HttpClient);
    private readonly EndPoint_Tag = "http://localhost:5000/api/v1/tags";

     // Métodos para consumir as APIs
    GetTag(): Observable<TagModel[]>{
        return this.http.get<TagModel[]>(this.EndPoint_Tag);
    }

    PostTag(Dados: TagCadastroModel): Observable<void>{

        return this.http.post<void>(this.EndPoint_Tag, Dados);
    }

}
