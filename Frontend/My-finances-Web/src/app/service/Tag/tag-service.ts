import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {TagModel} from '../../models/InterfaceModel'
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class TagService {
    private http = inject(HttpClient);
    private readonly Url = "http://localhost:5000/api/v1/tags";

     // Métodos para consumir as APIs
    getTag(): Observable<TagModel[]>{
        return this.http.get<TagModel[]>(this.Url);
    }

}
