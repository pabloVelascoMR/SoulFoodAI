import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeekReportService {
  
  private readonly apiUrl = 'https://api-soulfoodai.azurewebsites.net/api';

  constructor(private readonly http: HttpClient) {}

  submitReport(reportData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/UserDiary/SubmitReportAndCreatePlan`, reportData);
  }
}