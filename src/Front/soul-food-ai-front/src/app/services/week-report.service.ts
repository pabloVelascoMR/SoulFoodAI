import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeekReportService {
  
  private apiUrl = 'https://localhost:7007/api'; 

  constructor(private http: HttpClient) {}

  submitReport(reportData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/UserDiary/SubmitReportAndCreatePlan`, reportData);
  }
}