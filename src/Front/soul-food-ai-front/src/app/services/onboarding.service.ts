import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OnboardingService {
 
  private apiUrl = 'https://localhost:7007/api';

  constructor(private http: HttpClient) { }

  getGoals(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Goal/GetAllGoals`);
  }

  getIntolerances(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Intolerance/GetAllIntolerances`);
  }

  getFoodPlans(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/FoodPlan/GetAllFoodPlan`);
  }

  saveUserData(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/UserData/AddUserData`, userData);
  }
}