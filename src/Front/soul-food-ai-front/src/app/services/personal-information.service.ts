import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PersonalInformationService {
  
  private apiUrl = 'https://api-soulfoodai.azurewebsites.net/api/Ingredient';

  constructor(private http: HttpClient) {}

 
  getAllUserDatas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/UserData/GetAllUserDatas`);
  }

  getGoals(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Goal/GetAllGoals`);
  }

  getIntolerances(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Intolerance/GetAllIntolerances`);
  }

  getFoodPlans(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/FoodPlan/GetAllFoodPlan`);
  }

  updateUserData(userData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/UserData/EditUserData`, userData);
  }
  updateBodyMeasures(measures: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/UserData/UpdateBodyMeasures`, measures);
  }

  getUserDataById(userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/UserData/GetUserDataById/${userId}`);
  }
}