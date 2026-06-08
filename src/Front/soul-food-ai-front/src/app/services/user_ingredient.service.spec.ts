import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UserIngredientService } from './user_ingredient.service';

describe('UserIngredientService', () => {
  let service: UserIngredientService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        UserIngredientService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(UserIngredientService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería seleccionar un ingrediente', () => {
    service.selectIngredient(1, { idIngredient: 10, name: 'Pollo' }).subscribe();
    const reqAdd = httpMock.expectOne(req => req.url.includes('/AddFavorite'));
    expect(reqAdd.request.method).toBe('POST');
    reqAdd.flush({});
  });

  it('debería deseleccionar un ingrediente', () => {
    service.deselectIngredient(1, 10).subscribe();
    const reqRemove = httpMock.expectOne(req => req.url.includes('/RemoveFavorite/10/1'));
    expect(reqRemove.request.method).toBe('DELETE');
    reqRemove.flush({});
  });
});