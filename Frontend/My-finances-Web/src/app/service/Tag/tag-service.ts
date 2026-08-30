import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Tag, TagCadastro } from '../../models/tag.model';
import { Observable, startWith } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})

export class TagService {
    private http = inject(HttpClient);
    private readonly EndPoint_Tag = `${environment.apiUrl}/tags`;

     // Métodos para consumir as APIs
    GetTag(): Observable<Tag[]>{
        return this.http.get<Tag[]>(this.EndPoint_Tag).pipe(startWith([] as Tag[]));
    }

    PostTag(Dados: TagCadastro): Observable<void>{

        return this.http.post<void>(this.EndPoint_Tag, Dados);
    }

}
