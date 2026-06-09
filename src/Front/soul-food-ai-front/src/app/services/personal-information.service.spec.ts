import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { PersonalInformationService } from './personal-information.service';

describe('PersonalInformationService', () => {
  let service: PersonalInformationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        PersonalInformationService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(PersonalInformationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAllUserDatas should return data', () => {
    service.getAllUserDatas().subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserData/GetAllUserDatas');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getGoals should return data', () => {
    service.getGoals().subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/Goal/GetAllGoals');
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

  it('updateUserData should put data', () => {
    service.updateUserData({}).subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserData/EditUserData');
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('updateBodyMeasures should put data', () => {
    service.updateBodyMeasures({}).subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserData/UpdateBodyMeasures');
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('getUserDataById should get data', () => {
    service.getUserDataById(1).subscribe(res => expect(res).toBeTruthy());
    const req = httpMock.expectOne('https://api-soulfoodai.azurewebsites.net/api/UserData/GetUserDataById/1');
    expect(req.request.method).toBe('GET');
    req.flush({});
  });
});
