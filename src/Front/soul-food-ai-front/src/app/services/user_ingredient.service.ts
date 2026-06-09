import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserIngredientService {
  private readonly apiUrl = 'https://api-soulfoodai.azurewebsites.net/api/UserIngredient';

  constructor(private readonly http: HttpClient) {}
  getSelectedIngredients(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetFavorites/${userId}`);
  }

  selectIngredient(userId: number, ingredient: any): Observable<any> {
    const id = ingredient.idIngredient || ingredient.id;

    const payload = {
      idUser: userId,
      localIdIngredient: (id && id > 0) ? Number(id) : null,
      idOpenFoodFacts: ingredient.openFoodFactsId || null,
      name: ingredient.name || '',
      brand: ingredient.brand || '',
      imageUrl: ingredient.imageUrl || '',
      category: ingredient.category || '', 
      protein: ingredient.protein || 0,
      carbs: ingredient.carbs || 0,
      fat: ingredient.fat || 0,
      kcal: ingredient.kcal || 0
    };

    return this.http.post(`${this.apiUrl}/AddFavorite`, payload);
  }

  deselectIngredient(userId: number, ingredientId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/RemoveFavorite/${ingredientId}/${userId}`);
  }
}