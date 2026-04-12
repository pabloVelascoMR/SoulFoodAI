import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7007/api/User'; 

  constructor(private http: HttpClient) {}

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddUser`, user).pipe(
      tap((res: any) => this.saveSession(res.idUser))
    );
  }

  login(loginData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Login`, loginData).pipe(
      tap((res: any) => this.saveSession(res.idUser))
    );
  }

  private saveSession(userId: number): void {
    localStorage.setItem('soulfood_userId', userId.toString());
  }

  getUserId(): number | null {
    const id = localStorage.getItem('soulfood_userId');
    return id ? parseInt(id, 10) : null;
  }

  logout(): void {
    localStorage.removeItem('soulfood_userId');
  }
}