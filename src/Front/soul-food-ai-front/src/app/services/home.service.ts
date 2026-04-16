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
  targetKcal: number;
  realKcal: number;
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
    return this.http.get<DailyHeaderDto>(`${this.baseUrl}/UserFoodPlanWeek/GetDailyHeader/${idDaily}`);
  }

  getRecipesForUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Recipe/GetRecipesForUser/${userId}`);
  }

  updateDailyRecipes(data: { idUserFoodPlanDaily: number, recipeIds: number[] }): Observable<any> {
    return this.http.post(`${this.baseUrl}/UserFoodPlanWeek/UpdateDailyRecipes`, data);
  }
}