import { vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { FoodplanHistoryComponent } from './foodplan-history.component';
import { FoodplanHistoryService } from '../../services/foodplan-history.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('FoodplanHistoryComponent', () => {
  let component: FoodplanHistoryComponent;
  let historyMock: any;
  let userMock: any;

  beforeEach(async () => {
    historyMock = {
      getPlanHistory: () => of([]),
      hidePlanFromHistory: () => of({})
    };
    userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [FoodplanHistoryComponent],
      providers: [
        provideRouter([]),
        { provide: FoodplanHistoryService, useValue: historyMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(FoodplanHistoryComponent);
    component = fixture.componentInstance;
  });

  it('debería inicializar el componente y cargar historial si hay usuario', () => {
    const mockData = [{
      idUserFoodPlanWeek: 1,
      startDate: new Date().toISOString(),
      recipesEaten: [{ dateEaten: new Date().toISOString() }]
    }];
    vi.spyOn(historyMock, 'getPlanHistory').mockReturnValue(of(mockData));
    
    component.ngOnInit();
    expect(component.historyPlans.length).toBe(1);
    expect(component.isLoading).toBe(false);
  });

  it('debería mostrar error si no hay usuario', () => {
    vi.spyOn(userMock, 'getUserId').mockReturnValue(null);
    component.ngOnInit();
    expect(component.errorMessage).toContain('No se ha detectado una sesión activa');
  });

  it('debería manejar error 404 de getPlanHistory', () => {
    vi.spyOn(historyMock, 'getPlanHistory').mockReturnValue(throwError(() => ({ status: 404 })));
    component.ngOnInit();
    expect(component.errorMessage).toContain('No tienes historial de planes');
  });

  it('debería manejar otro error de getPlanHistory', () => {
    vi.spyOn(historyMock, 'getPlanHistory').mockReturnValue(throwError(() => ({ status: 500 })));
    component.ngOnInit();
    expect(component.errorMessage).toContain('Hubo un error al conectar');
  });

  it('debería confirmar y cancelar ocultar plan', () => {
    const mockEvent = new Event('click');
    component.confirmHide(1, mockEvent);
    expect(component.planToHide).toBe(1);

    component.cancelHide();
    expect(component.planToHide).toBeNull();
  });

  it('debería ejecutar ocultar plan exitosamente', () => {
    component.historyPlans = [{ idUserFoodPlanWeek: 1 }];
    component.planToHide = 1;
    vi.spyOn(historyMock, 'hidePlanFromHistory').mockReturnValue(of({}));
    
    component.executeHide();
    expect(component.historyPlans.length).toBe(0);
    expect(component.planToHide).toBeNull();
    expect(component.errorMessage).toContain('No tienes historial');
  });

  it('debería manejar error al ejecutar ocultar plan', () => {
    vi.spyOn(globalThis, 'alert');
    component.planToHide = 1;
    vi.spyOn(historyMock, 'hidePlanFromHistory').mockReturnValue(throwError(() => new Error('Simulated error')));
    
    component.executeHide();
    expect(globalThis.alert).toHaveBeenCalledWith('No se pudo ocultar el plan. Inténtalo de nuevo.');
    expect(component.planToHide).toBeNull();
  });

  it('debería obtener la clase de la comida correctamente', () => {
    expect(component.getMealClass('Desayuno de prueba')).toBe('meal-yellow');
    expect(component.getMealClass('Comida rica')).toBe('meal-orange');
    expect(component.getMealClass('Cena ligera')).toBe('meal-blue');
    expect(component.getMealClass('Snack')).toBe('meal-purple');
    expect(component.getMealClass('Random')).toBe('meal-red');
    expect(component.getMealClass('')).toBe('');
  });
});


