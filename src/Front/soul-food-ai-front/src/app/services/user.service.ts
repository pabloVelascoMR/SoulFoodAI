import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7007/api/User'; 
  private authUrl = 'https://localhost:7007/api/Auth'; 

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddUser`, user).pipe(
      tap((res: any) => {
        if (isPlatformBrowser(this.platformId)) {
          const id = res.idUser || res.IdUser; 
          if (id) {
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
    if (isPlatformBrowser(this.platformId)) {
      const id = localStorage.getItem('soulfood_userId');
      return id ? parseInt(id, 10) : null;
    }
    return null;
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('soulfood_userId');
      localStorage.removeItem('soulfood_token');
    }
  }
}