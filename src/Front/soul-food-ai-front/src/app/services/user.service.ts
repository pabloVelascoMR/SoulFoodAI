import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7007/api/User'; 
  private authUrl = 'https://localhost:7007/api/Auth'; 

  constructor(private http: HttpClient) {}

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddUser`, user).pipe(
      tap((res: any) => {
        const id = res.idUser || res.IdUser; 
        if (id) {
          localStorage.setItem('soulfood_userId', id.toString());
        }

        const token = res.token || res.Token; 
        if (token) {
          localStorage.setItem('soulfood_token', token);
        }
      })
    );
  }

  login(loginData: any): Observable<any> {
    
    return this.http.post(`${this.authUrl}/Login`, loginData).pipe(
      tap((res: any) => {
        // Guardamos la sesión leyendo exactamente lo que devuelve tu AuthController
        const id = res.idUser || res.IdUser; 
        if (id) {
          localStorage.setItem('soulfood_userId', id.toString());
        }

        const token = res.token || res.Token; 
        if (token) {
          localStorage.setItem('soulfood_token', token);
        }
      })
    );
  }

  getUserId(): number | null {
    const id = localStorage.getItem('soulfood_userId');
    return id ? parseInt(id, 10) : null;
  }

  logout(): void {
    localStorage.removeItem('soulfood_userId');
    localStorage.removeItem('soulfood_token');
  }
}