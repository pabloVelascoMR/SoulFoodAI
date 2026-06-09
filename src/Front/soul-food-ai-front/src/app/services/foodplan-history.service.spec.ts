import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { FoodplanHistoryService } from './foodplan-history.service';

describe('FoodplanHistoryService', () => {
  let service: FoodplanHistoryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        FoodplanHistoryService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(FoodplanHistoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getPlanHistory should return data', () => {
    service.getPlanHistory(1).subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserFoodPlanWeek/GetPlanHistory/1');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('hidePlanFromHistory should put data', () => {
    service.hidePlanFromHistory(1).subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserFoodPlanWeek/HidePlanFromHistory/1');
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });
});
