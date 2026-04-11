import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

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
  
}