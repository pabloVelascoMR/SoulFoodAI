import { vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { IngredientSelectionComponent } from './ingredient-selection.component';
import { IngredientService } from '../../services/ingredient.service';
import { UserIngredientService } from '../../services/user_ingredient.service';
import { UserService } from '../../services/user.service';
import { provideRouter, Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { ChangeDetectorRef } from '@angular/core';

describe('IngredientSelectionComponent', () => {
  let component: IngredientSelectionComponent;
  let ingredientMock: any;
  let userIngredientMock: any;
  let userMock: any;
  let router: any;

  beforeEach(async () => {
    ingredientMock = {
      getIngredients: () => of([]),
      deleteCustomIngredient: () => of({}),
      updateCustomIngredient: () => of({}),
      addCustomIngredient: () => of({}),
      searchOpenFoodFacts: () => of([]),
      addSearchedIngredient: () => of({})
    };
    userIngredientMock = {
      getSelectedIngredients: () => of([{ idIngredient: 1 }]),
      selectIngredient: () => of({}),
      deselectIngredient: () => of({})
    };
    userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [IngredientSelectionComponent, HttpClientTestingModule],
      providers: [
        provideRouter([]),
        { provide: IngredientService, useValue: ingredientMock },
        { provide: UserIngredientService, useValue: userIngredientMock },
        { provide: UserService, useValue: userMock },
        ChangeDetectorRef
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(IngredientSelectionComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');
  });

  it('debería inicializar y cargar datos', () => {
    vi.spyOn(component, 'loadUserDataAndSetup');
    component.ngOnInit();
    expect(component.userId).toBe(1);
    expect(component.loadUserDataAndSetup).toHaveBeenCalled();
  });

  it('debería redirigir a login si no hay usuario', () => {
    vi.spyOn(userMock, 'getUserId').mockReturnValue(null);
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería manejar configuración de dietas vegana/vegetariana', () => {
    component.userDietType = '7'; // Vegana
    component.setupSteps();
    expect(component.steps.find(s => s.name === 'Carne')).toBeUndefined();
    expect(component.requiredMinimum).toBe(20);

    component.userDietType = '8'; // Vegetariana
    component.setupSteps();
    expect(component.steps.find(s => s.name === 'Carne')).toBeUndefined();
    expect(component.requiredMinimum).toBe(25);

    component.userDietType = '9'; // Pescatariana
    component.setupSteps();
    expect(component.steps.find(s => s.name === 'Carne')).toBeUndefined();
    expect(component.requiredMinimum).toBe(30);

    component.userDietType = '1'; // General
    component.setupSteps();
    expect(component.steps.find(s => s.name === 'Carne')).toBeDefined();
    expect(component.requiredMinimum).toBe(35);
  });

  it('debería alternar seleccion de ingrediente', () => {
    component.userId = 1;
    component.selectedIngredientIds = new Set([1]);
    
    component.toggleIngredientSelection({ idIngredient: 1 });
    expect(component.selectedIngredientIds.has(1)).toBe(false);

    component.toggleIngredientSelection({ idIngredient: 2 });
    expect(component.selectedIngredientIds.has(2)).toBe(true);
  });

  it('debería comprobar si puede finalizar', () => {
    vi.spyOn(component, 'requiredMinimum', 'get').mockReturnValue(5);
    component.selectedIngredientIds = new Set([1, 2, 3]);
    expect(component.canFinish).toBe(false);
    
    component.selectedIngredientIds = new Set([1, 2, 3, 4, 5]);
    expect(component.canFinish).toBe(true);
  });

  it('debería finalizar y navegar o alertar', () => {
    vi.spyOn(component, 'canFinish', 'get').mockReturnValue(true);
    component.finalizar();
    expect(router.navigate).toHaveBeenCalledWith(['/home']);

    vi.spyOn(window, 'alert');
    vi.spyOn(component, 'canFinish', 'get').mockReturnValue(false); // Override previous spy
    component.finalizar();
    expect(window.alert).toHaveBeenCalled();
  });

  it('debería cambiar de paso', () => {
    component.steps = [{name: 'A'}, {name: 'B'}];
    component.currentStepIndex = 0;
    
    component.nextStep();
    expect(component.currentStepIndex).toBe(1);
    
    component.previousStep();
    expect(component.currentStepIndex).toBe(0);
  });

  it('debería gestionar imagen de ingredientes', () => {
    expect(component.getIngredientImage({ id: 1 })).toContain('/assets/');
    expect(component.getIngredientImage({ id: 999, imageUrl: 'http://img' })).toBe('http://img');
    
    const ingredient = { hasImageError: false };
    component.handleImageError({ target: { style: {} } }, ingredient);
    expect(ingredient.hasImageError).toBe(true);
  });

  it('debería permitir edición o borrado de custom', () => {
    component.userId = 1;
    const ing1 = { createdByUserId: 1, idIngredient: 200 };
    expect(component.canEdit(ing1)).toBe(true);
    expect(component.canDelete(ing1)).toBe(true);

    const ing2 = { createdByUserId: 1, idIngredient: 100 }; // ID < 191
    expect(component.canDelete(ing2)).toBe(false);

    const ing3 = { createdByUserId: 1, idOpenFoodFacts: 'xxx' };
    expect(component.canEdit(ing3)).toBe(false);
  });

  it('debería borrar ingrediente custom', () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const mockEvent = new Event('click');
    component.deleteCustomIngredient({ id: 200, name: 'Test' }, mockEvent);
    expect(ingredientMock.deleteCustomIngredient).toBeTruthy();
  });

  it('debería abrir modales de edición custom', () => {
    component.steps = [{ name: 'Test' }];
    component.openCustomModal();
    expect(component.showCustomModal).toBe(true);
    expect(component.newIngredient.category).toBe('Test');

    const mockEvent = new Event('click');
    component.editCustomIngredient({ id: 200 }, mockEvent);
    expect(component.isEditing).toBe(true);
    expect(component.showCustomModal).toBe(true);
  });

  it('debería guardar ingrediente custom', () => {
    component.newIngredient = { name: 'Test', id: 200 };
    component.isEditing = true;
    component.saveCustomIngredient();
    expect(component.showCustomModal).toBe(false);

    component.isEditing = false;
    component.saveCustomIngredient();
    expect(component.showCustomModal).toBe(false);
  });

  it('debería buscar en OFF', () => {
    component.offSearchQuery = 'Test';
    component.searchOFF();
    expect(component.isSearchingOFF).toBe(false);
    expect(component.offSearchResults).toBeDefined();
  });

  it('debería seleccionar en OFF', () => {
    component.steps = [{ name: 'Test' }];
    component.selectOFFIngredient({ openFoodFactsId: '123' });
    expect(component.showOFFModal).toBe(false);
  });
});



