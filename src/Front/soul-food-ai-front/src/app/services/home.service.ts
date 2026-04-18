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

export interface WeekCalendarDto {
  idUserFoodPlanWeek: number;
  mealsPerDay: number;
  days: any[];
}

export interface DailyHeaderDto {
  idUserFoodPlanDaily: number;
  dietName: string;       // <-- Añadido de tu C#
  dayName: string;        // <-- Añadido de tu C#
  targetKcal: number;
  realKcal: number;
  targetProtein: number;
  realProtein: number;
  targetCarbs: number;
  realCarbs: number;
  targetFat: number;
  realFat: number;
  mealsPerDay: number;    // <-- Añadido de tu C#
}

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  private baseUrl = 'https://localhost:7007/api';

  constructor(private http: HttpClient) { }

  getWeeklyHeader(userId: number): Observable<WeeklyHeaderDto> {
    return this.http.get<WeeklyHeaderDto>(`${this.baseUrl}/UserFoodPlanWeek/GetWeeklyHeader/${userId}`);
  }

  getActiveWeekCalendar(userId: number): Observable<WeekCalendarDto> {
    return this.http.get<WeekCalendarDto>(`${this.baseUrl}/UserFoodPlanWeek/GetActiveWeekCalendar/${userId}`);
  }

  getDailyHeader(idDaily: number): Observable<DailyHeaderDto> {
    return this.http.get<DailyHeaderDto>(`${this.baseUrl}/UserFoodPlanDaily/GetDailyHeader/${idDaily}`);
  }

  getRecipesForUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Recipe/GetRecipesForUser/${userId}`);
  }

  updateDailyRecipes(data: { idUserFoodPlanDaily: number, recipeIds: number[] }): Observable<any> {
    return this.http.post(`${this.baseUrl}/UserFoodPlanWeek/UpdateDailyRecipes`, data);
  }

  createWeeklyPlan(userId: number): Observable<any> {
    // CAMBIO AQUÍ: Usamos GenerateWeekPlan exactamente como está en C#
    return this.http.post(`${this.baseUrl}/UserFoodPlanWeek/GenerateWeekPlan/${userId}`, {});
  }

  adjustDayMacros(idDaily: number): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/UserFoodPlanDaily/AdjustDayMacros/${idDaily}`, {});
  }
}