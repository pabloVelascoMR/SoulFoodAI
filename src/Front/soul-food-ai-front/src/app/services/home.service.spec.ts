import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { HomeService } from './home.service';

describe('HomeService', () => {
  let service: HomeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        HomeService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(HomeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería obtener la cabecera de la semana', () => {
    service.getWeeklyHeader(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GetWeeklyHeader'));
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('debería actualizar las recetas del día', () => {
    service.updateDailyRecipes({ idUserFoodPlanDaily: 1, recipeIds: [1] }).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/UpdateDailyRecipes'));
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('debería ajustar los macros usando la IA', () => {
    service.adjustDayMacros(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/AdjustDayMacros'));
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('debería obtener el calendario activo', () => {
    service.getActiveWeekCalendar(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GetActiveWeekCalendar/1'));
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('debería obtener el daily header', () => {
    service.getDailyHeader(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GetDailyHeader/1'));
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('debería obtener recetas para usuario', () => {
    service.getRecipesForUser(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GetRecipesForUser/1'));
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('debería crear un plan semanal', () => {
    service.createWeeklyPlan(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GenerateWeekPlan/1'));
    expect(req.request.method).toBe('POST');
    req.flush({});
  });
});