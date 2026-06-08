import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { WeekReportService } from './week-report.service';

describe('WeekReportService', () => {
  let service: WeekReportService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        WeekReportService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(WeekReportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería enviar el reporte', () => {
    service.submitReport({ hungerLevel: 5 }).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/SubmitReportAndCreatePlan'));
    expect(req.request.method).toBe('POST');
    req.flush({});
  });
});