import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateAiRecipeDto {
  promptDescription?: string;
  idMeal: number;
}

@Injectable({
  providedIn: 'root'
})
export class RecipesService {
  private apiUrl = 'https://localhost:7007/api'; 

  constructor(private http: HttpClient) { }

  generateRecipeAI(idUser: number, dto: CreateAiRecipeDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/Recipe/CreateRecipeAI/${idUser}`, dto);
  }

  getMeals(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Meal/GetAllMeals`);
  }
}