import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Ingredient {
  idIngredient?: number; 
  openFoodFactsId?: string;
  name: string;
  brand?: string;      
  imageUrl?: string;
  category?: string;
  icon?: string;
  protein?: number;
  carbs?: number;
  fat?: number;
  kcal?: number;
}

@Injectable({
  providedIn: 'root'
})
export class IngredientService {
  private apiUrl = 'https://api-soulfoodai.azurewebsites.net/api/Ingredient';

  constructor(private http: HttpClient) {}
  
  getIngredients(category: string, userId: number): Observable<any[]> {
    const params = new HttpParams()
      .set('category', category)
      .set('userId', userId.toString());

    return this.http.get<any[]>(`${this.apiUrl}/GetIngredients`, { params });
  }

  addCustomIngredient(ingredientData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddCustomIngredient`, ingredientData);
  }

  deleteCustomIngredient(id: number, userId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/DeleteCustomIngredient/${id}/${userId}`);
  }

  updateCustomIngredient(id: number, ingredientData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/UpdateCustomIngredient/${id}`, ingredientData);
  }
  
  searchOpenFoodFacts(query: string): Observable<any[]> {
    const url = `${this.apiUrl}/SearchOFFIngredients?searchText=${encodeURIComponent(query)}`;
    return this.http.get<any[]>(url);
  }

  addSearchedIngredient(ingredientData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddSearchedIngredient`, ingredientData);
  }
}