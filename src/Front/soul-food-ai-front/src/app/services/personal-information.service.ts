import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PersonalInformationService {
  private apiUrl = 'https://localhost:7007/api'; // Asegúrate de que apunte a HTTPS si arreglaste el CORS así

  constructor(private http: HttpClient) {}

  // NUEVO: Traemos todos los perfiles para filtrarlos luego en el componente
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
}