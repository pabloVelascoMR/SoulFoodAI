import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RecipeEaten {
  idRecipe: number;
  recipeName: string;
  mealType: string;
  dateEaten: Date;
}

export interface PlanHistory {
  idUserFoodPlanWeek: number;
  dietName: string;
  startDate: Date;
  endDate: Date;
  isActive: boolean;
  recipesEaten: RecipeEaten[];
}

@Injectable({
  providedIn: 'root'
})
export class FoodplanHistoryService {
  private apiUrl = 'https://localhost:7007/api';

  constructor(private http: HttpClient) { }

  getPlanHistory(idUser: number): Observable<PlanHistory[]> {
    
    return this.http.get<PlanHistory[]>(`${this.apiUrl}/UserFoodPlanWeek/GetPlanHistory/${idUser}`);
  }

  hidePlanFromHistory(idUserFoodPlanWeek: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/UserFoodPlanWeek/HidePlanFromHistory/${idUserFoodPlanWeek}`, {});
  }
}