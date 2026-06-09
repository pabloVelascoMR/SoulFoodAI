import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { OnboardingService } from './onboarding.service';

describe('OnboardingService', () => {
  let service: OnboardingService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        OnboardingService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(OnboardingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getGoals should return data', () => {
    service.getGoals().subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/Goal/GetAllGoals');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getIntolerances should return data', () => {
    service.getIntolerances().subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/Intolerance/GetAllIntolerances');
    req.flush([]);
  });

  it('getFoodPlans should return data', () => {
    service.getFoodPlans().subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/FoodPlan/GetAllFoodPlan');
    req.flush([]);
  });

  it('saveUserData should post data', () => {
    service.saveUserData({}).subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserData/AddUserData');
    expect(req.request.method).toBe('POST');
    req.flush({});
  });
});
