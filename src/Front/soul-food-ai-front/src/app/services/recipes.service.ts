import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateAiRecipeDto {
  promptDescription?: string;
  idMeal: number;
}

export interface AddRecipeDto {
  idMeal: number;
  recipeName: string;
  recipeDescription: string;
  idIngredients: number[];
  quantity: number[];
  unit: string[];
}

@Injectable({
  providedIn: 'root'
})
export class RecipesService {

  private readonly apiUrl = 'https://api-soulfoodai.azurewebsites.net/api'; 

  constructor(private readonly http: HttpClient) { }

  generateRecipeAI(idUser: number, dto: CreateAiRecipeDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/Recipe/CreateRecipeAI/${idUser}`, dto);
  }

  addRecipeManual(idUser: number, dto: AddRecipeDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/Recipe/AddRecipesForUser/${idUser}`, dto);
  }

  getUserRecipes(idUser: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Recipe/GetRecipesForUser/${idUser}`);
  }

  getMeals(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Meal/GetAllMeals`);
  }

  getAllowedIngredients(idUser: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Ingredient/GetAllowedIngredients/${idUser}`);
  }

  archiveRecipe(idRecipe: number) {
  return this.http.put(`${this.apiUrl}/Recipe/ArchiveRecipe/${idRecipe}`, {});
}
}