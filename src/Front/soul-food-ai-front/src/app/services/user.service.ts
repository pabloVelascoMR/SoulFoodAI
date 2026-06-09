import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = 'https://api-soulfoodai.azurewebsites.net/api/User';
  private readonly authUrl = 'https://api-soulfoodai.azurewebsites.net/api/Auth';
  private userId: number | null = null;

  constructor(private readonly http: HttpClient,
    @Inject(PLATFORM_ID) private readonly platformId: Object
  ) {}

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddUser`, user).pipe(
      tap((res: any) => {
        if (isPlatformBrowser(this.platformId)) {
          const id = res.idUser || res.IdUser; 
          if (id) {
            this.userId = Number.parseInt(id, 10);
            localStorage.setItem('soulfood_userId', id.toString());
          }

          const token = res.token || res.Token; 
          if (token) {
            localStorage.setItem('soulfood_token', token);
          }
        }
      })
    );
  }

  login(loginData: any): Observable<any> {
    return this.http.post(`${this.authUrl}/Login`, loginData).pipe(
      tap((res: any) => {
        if (isPlatformBrowser(this.platformId)) {
          const id = res.idUser || res.IdUser; 
          if (id) {
            this.userId = Number.parseInt(id, 10);
            localStorage.setItem('soulfood_userId', id.toString());
          }

          const token = res.token || res.Token; 
          if (token) {
            localStorage.setItem('soulfood_token', token);
          }
        }
      })
    );
  }

  getUserId(): number | null {
    if (this.userId) {
      return this.userId;
    }

    if (isPlatformBrowser(this.platformId)) {
      const id = localStorage.getItem('soulfood_userId');
      if (id) {
        this.userId = Number.parseInt(id, 10);
        return this.userId;
      }
    }
    
    return null;
  }

  logout(): void {
    this.userId = null;
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('soulfood_userId');
      localStorage.removeItem('soulfood_token');
    }
  }
}