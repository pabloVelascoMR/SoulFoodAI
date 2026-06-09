import { vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { RecipesComponent } from './recipes.component';
import { RecipesService } from '../../services/recipes.service';
import { UserService } from '../../services/user.service';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { PLATFORM_ID, ChangeDetectorRef } from '@angular/core';

describe('RecipesComponent', () => {
  let component: RecipesComponent;
  let recipesMock: any;
  let userMock: any;
  let router: any;

  beforeEach(async () => {
    recipesMock = {
      getMeals: () => of([]),
      getAllowedIngredients: () => of([]),
      getUserRecipes: () => of([]),
      generateRecipeAI: () => of({ message: 'Success' }),
      addRecipeManual: () => of({}),
      archiveRecipe: () => of({})
    };
    userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [RecipesComponent],
      providers: [
        provideRouter([]),
        { provide: RecipesService, useValue: recipesMock },
        { provide: UserService, useValue: userMock },
        { provide: PLATFORM_ID, useValue: 'browser' },
        ChangeDetectorRef
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(RecipesComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');
  });

  it('debería inicializar datos y cargar recetas', () => {
    vi.spyOn(component, 'loadInitialData');
    component.ngOnInit();
    expect(component.idUser).toBe(1);
    expect(component.loadInitialData).toHaveBeenCalled();
  });

  it('debería redirigir a login si no hay usuario', () => {
    vi.spyOn(userMock, 'getUserId').mockReturnValue(null);
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería manejar modales de info y ai y manual', () => {
    component.openAiModal();
    expect(component.isAiModalOpen).toBe(true);
    component.closeAiModal();
    expect(component.isAiModalOpen).toBe(false);

    component.openManualModal();
    expect(component.isManualModalOpen).toBe(true);
    component.closeManualModal();
    expect(component.isManualModalOpen).toBe(false);

    component.openInfoModal({});
    expect(component.isInfoModalOpen).toBe(true);
    component.closeInfoModal();
    expect(component.isInfoModalOpen).toBe(false);
  });

  it('debería manejar modal de borrado', () => {
    const mockEvent = new Event('click');
    component.openDeleteModal(1, mockEvent);
    expect(component.isDeleteModalOpen).toBe(true);
    expect(component.recipeToDeleteId).toBe(1);

    component.closeDeleteModal();
    expect(component.isDeleteModalOpen).toBe(false);
  });

  it('debería agregar y quitar ingredientes', () => {
    component.manualRecipe.ingredients = [];
    component.addIngredientRow();
    expect(component.manualRecipe.ingredients.length).toBe(1);
    component.removeIngredientRow(0);
    expect(component.manualRecipe.ingredients.length).toBe(0);
  });

  it('debería obtener clases de comida', () => {
    expect(component.getMealClass('Desayuno')).toBe('meal-desayuno');
    expect(component.getMealClass('Almuerzo')).toBe('meal-almuerzo');
    expect(component.getMealClass('Comida')).toBe('meal-comida');
    expect(component.getMealClass('Merienda')).toBe('meal-merienda');
    expect(component.getMealClass('Cena')).toBe('meal-cena');
    expect(component.getMealClass('Random')).toBe('');
  });

  it('debería generar receta AI exitosamente', () => {
    component.selectedMealAi = 1;
    component.promptTextAi = 'Pollo';
    component.generateAiRecipe();
    expect(component.isLoadingAi).toBe(false);
    expect(component.successAi).toBe('Success');
  });

  it('debería fallar generación AI sin comida seleccionada', () => {
    component.selectedMealAi = null;
    component.generateAiRecipe();
    expect(component.errorAi).toContain('Selecciona');
  });

  it('debería manejar error en generación AI', () => {
    component.selectedMealAi = 1;
    vi.spyOn(recipesMock, 'generateRecipeAI').mockReturnValue(throwError(() => ({ error: { message: 'AI Error' } })));
    component.generateAiRecipe();
    expect(component.errorAi).toBe('AI Error');
  });

  it('debería subir receta manual exitosamente', () => {
    component.manualRecipe = { recipeName: 'Test', recipeDescription: 'Test', idMeal: 1, ingredients: [{ idIngredient: 1, quantity: 100, unit: 'g' }] };
    component.submitManualRecipe();
    expect(component.successManual).toBe('Receta guardada.');
  });

  it('debería fallar receta manual si faltan datos', () => {
    component.manualRecipe = { recipeName: '', recipeDescription: '', idMeal: null, ingredients: [] };
    component.submitManualRecipe();
    expect(component.errorManual).toContain('Completa los campos');
  });

  it('debería manejar error en receta manual', () => {
    component.manualRecipe = { recipeName: 'Test', recipeDescription: 'Test', idMeal: 1, ingredients: [{ idIngredient: 1, quantity: 100, unit: 'g' }] };
    vi.spyOn(recipesMock, 'addRecipeManual').mockReturnValue(throwError(() => ({ error: 'Error manual' })));
    component.submitManualRecipe();
    expect(component.errorManual).toBe('Error manual');
  });

  it('debería archivar receta con confirmacion directa', () => {
    vi.spyOn(globalThis, 'confirm').mockReturnValue(true);
    const mockEvent = new Event('click');
    component.archiveRecipe(1, mockEvent);
    expect(recipesMock.archiveRecipe).toBeTruthy(); 
  });

  it('debería archivar receta desde el modal de borrado', () => {
    component.recipeToDeleteId = 1;
    component.confirmDeleteRecipe();
    expect(component.isDeleting).toBe(false);
    expect(component.isDeleteModalOpen).toBe(false);
  });

  it('debería manejar error al archivar receta desde modal', () => {
    vi.spyOn(globalThis, 'alert');
    component.recipeToDeleteId = 1;
    vi.spyOn(recipesMock, 'archiveRecipe').mockReturnValue(throwError(() => new Error('Simulated error')));
    component.confirmDeleteRecipe();
    expect(globalThis.alert).toHaveBeenCalled();
  });
});


