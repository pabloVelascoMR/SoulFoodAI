import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { IngredientService } from './ingredient.service';

describe('IngredientService', () => {
  let service: IngredientService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        IngredientService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(IngredientService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería obtener ingredientes', () => {
    service.getIngredients('Carne', 1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GetIngredients'));
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('debería buscar en OpenFoodFacts', () => {
    service.searchOpenFoodFacts('Tomate').subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/SearchOFFIngredients'));
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('debería añadir y eliminar ingredientes personalizados', () => {
    service.addCustomIngredient({ name: 'Avena' }).subscribe();
    const reqAdd = httpMock.expectOne(req => req.url.includes('/AddCustomIngredient'));
    expect(reqAdd.request.method).toBe('POST');
    reqAdd.flush({});

    service.deleteCustomIngredient(1, 1).subscribe();
    const reqDel = httpMock.expectOne(req => req.url.includes('/DeleteCustomIngredient'));
    expect(reqDel.request.method).toBe('DELETE');
    reqDel.flush({});
  });
});