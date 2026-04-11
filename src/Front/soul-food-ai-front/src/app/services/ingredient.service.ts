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
  private apiUrl = 'https://localhost:7007/api/Ingredient';

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
  
  searchOpenFoodFacts(query: string): Observable<any> {
    const url = `https://es.openfoodfacts.org/cgi/search.pl?search_terms=${query}&search_simple=1&action=process&json=1&page_size=20&fields=code,product_name,product_name_es,brands,image_url,nutriments`;
    return this.http.get(url);
  }

  addSearchedIngredient(ingredientData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddSearchedIngredient`, ingredientData);
  }
}