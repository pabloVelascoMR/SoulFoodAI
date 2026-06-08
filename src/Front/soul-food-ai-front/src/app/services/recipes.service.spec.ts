import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { RecipesService } from './recipes.service';

describe('RecipesService', () => {
  let service: RecipesService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        RecipesService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(RecipesService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería obtener recetas del usuario (getUserRecipes)', () => {
    service.getUserRecipes(1).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/GetRecipesForUser/1'));
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('debería añadir una receta manual (addRecipeManual)', () => {
    service.addRecipeManual(1, { recipeName: 'Pollo', idMeal: 1, recipeDescription: '', idIngredients: [], quantity: [], unit: [] }).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/AddRecipesForUser/1'));
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('debería generar una receta con IA (generateRecipeAI)', () => {
    service.generateRecipeAI(1, { idMeal: 1, promptDescription: 'Algo sano' }).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/CreateRecipeAI/1'));
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('debería archivar una receta (archiveRecipe)', () => {
    service.archiveRecipe(10).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/ArchiveRecipe/10'));
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });
});