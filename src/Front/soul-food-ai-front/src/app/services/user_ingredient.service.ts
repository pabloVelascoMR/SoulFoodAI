import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserIngredientService {
  private apiUrl = 'https://localhost:7007/api/UserIngredient';

  constructor(private http: HttpClient) {}
  
}