import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface WeeklyHeaderDto {
  idUserFoodPlanWeek: number;
  dietName: string;
  totalWeeklyKcal: number;
  targetProteinPercent: number;
  targetCarbsPercent: number;
  targetFatPercent: number;
}

export interface DailyHeaderDto {
  idUserFoodPlanDaily: number;
  dietName: string;
  dayName: string;
  targetKcal: number;
  realKcal: number;
  targetProtein: number;
  realProtein: number;
  targetCarbs: number;
  realCarbs: number;
  targetFat: number;
  realFat: number;
  mealsPerDay: number;
}

export interface DayCalendarDto {
  idUserFoodPlanDaily: number;
  dayName: string;
  dateNumber: string;
  fullDate: string;
  assignedRecipes: any[]; 
}

export interface WeekCalendarDto {
  idUserFoodPlanWeek: number;
  mealsPerDay: number;
  days: DayCalendarDto[];
}

export interface RecipeCardDto {
  idRecipe: number;
  recipeName: string;
  kcal: number;
  mealName: string;
}

export interface UpdateDailyRecipesDto {
  idUserFoodPlanDaily: number;
  recipeIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  private baseUrl = 'https://localhost:7007/api'; 

  constructor(private http: HttpClient) {}

  getWeeklyHeader(idUser: number): Observable<WeeklyHeaderDto> {
    return this.http.get<WeeklyHeaderDto>(`${this.baseUrl}/UserFoodPlanWeek/GetWeeklyHeader/${idUser}`);
  }

  getActiveWeekCalendar(idUser: number): Observable<WeekCalendarDto> {
    return this.http.get<WeekCalendarDto>(`${this.baseUrl}/UserFoodPlanWeek/GetActiveWeekCalendar/${idUser}`);
  }

  getDailyHeader(idDailyPlan: number): Observable<DailyHeaderDto> {
    return this.http.get<DailyHeaderDto>(`${this.baseUrl}/UserFoodPlanDaily/GetDailyHeader/${idDailyPlan}`);
  }

  getRecipesForUser(idUser: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Recipe/GetRecipesForUser/${idUser}`);
  }

  updateDailyRecipes(dto: UpdateDailyRecipesDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/UserFoodPlanDaily/UpdateDailyRecipes`, dto);
  }
}