import { vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { HomeService } from '../../services/home.service';
import { UserService } from '../../services/user.service';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ChangeDetectorRef } from '@angular/core';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let homeServiceMock: any;
  let userServiceMock: any;
  let router: any;

  beforeEach(async () => {
    homeServiceMock = {
      getActiveWeekCalendar: () => of({ days: [{ idUserFoodPlanDaily: 1, dayName: 'Lunes' }] }),
      getWeeklyHeader: () => of({}),
      getRecipesForUser: () => of([{ recipeName: 'Pollo' }, { recipeName: '[AJUSTADO] Pollo' }]),
      getDailyHeader: () => of({ realKcal: 0 }),
      adjustDayMacros: () => of({}),
      updateDailyRecipes: () => of({}),
      createWeeklyPlan: () => of({})
    };
    userServiceMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        provideRouter([]),
        { provide: HomeService, useValue: homeServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: PLATFORM_ID, useValue: 'browser' },
        ChangeDetectorRef
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
  });

  it('debería inicializar datos del dashboard', () => {
    vi.spyOn(component, 'loadDashboardData');
    vi.spyOn(component, 'loadDailyHeader');
    component.ngOnInit();
    expect(component.loadDashboardData).toHaveBeenCalled();
    expect(component.hasActivePlan).toBe(true);
    expect(component.availableRecipes.length).toBe(1); // Filtra [AJUSTADO]
  });

  it('debería manejar error al obtener calendario', () => {
    vi.spyOn(homeServiceMock, 'getActiveWeekCalendar').mockReturnValue(throwError(() => new Error()));
    component.loadDashboardData();
    expect(component.hasActivePlan).toBe(false);
    expect(component.loading).toBe(false);
  });

  it('debería manejar calendario vacío', () => {
    vi.spyOn(homeServiceMock, 'getActiveWeekCalendar').mockReturnValue(of({ days: [] }));
    component.loadDashboardData();
    expect(component.hasActivePlan).toBe(false);
    expect(component.loading).toBe(false);
  });

  it('debería ajustar macros del día seleccionado', () => {
    component.selectedDayId = 1;
    component.adjustMacrosForSelectedDay();
    expect(component.isAdjustingMacros).toBe(false);
    expect(component.aiMessageType).toBe('success');
  });

  it('debería manejar error timeout al ajustar macros', () => {
    component.selectedDayId = 1;
    vi.spyOn(homeServiceMock, 'adjustDayMacros').mockReturnValue(throwError(() => ({ name: 'TimeoutError' })));
    component.adjustMacrosForSelectedDay();
    expect(component.aiMessageType).toBe('error');
    expect(component.aiMessage).toContain('tardando demasiado');
  });

  it('debería manejar error 503 al ajustar macros', () => {
    component.selectedDayId = 1;
    vi.spyOn(homeServiceMock, 'adjustDayMacros').mockReturnValue(throwError(() => ({ status: 503 })));
    component.adjustMacrosForSelectedDay();
    expect(component.aiMessageType).toBe('error');
    expect(component.aiMessage).toContain('saturados');
  });

  it('debería manejar error al cargar recetas', () => {
    vi.spyOn(homeServiceMock, 'getRecipesForUser').mockReturnValue(throwError(() => new Error()));
    component.loadRemainingData();
    expect(component.loading).toBe(false);
  });

  it('debería seleccionar y limpiar día', () => {
    component.selectDay(5);
    expect(component.selectedDayId).toBe(5);
    component.clearSelectedDay();
    expect(component.selectedDayId).toBeNull();
  });

  it('debería obtener el nombre del día', () => {
    component.calendar = { idUserFoodPlanWeek: 1, mealsPerDay: 5, days: [{ idUserFoodPlanDaily: 1, dayName: 'Martes' }] };
    expect(component.getDayName(1)).toBe('Martes');
    expect(component.getDayName(99)).toBe('');
  });

  it('debería obtener la clase de la comida', () => {
    expect(component.getMealClass('Desayuno')).toBe('meal-desayuno');
    expect(component.getMealClass('Almuerzo')).toBe('meal-almuerzo');
    expect(component.getMealClass('Comida')).toBe('meal-comida');
    expect(component.getMealClass('Merienda')).toBe('meal-merienda');
    expect(component.getMealClass('Cena')).toBe('meal-cena');
    expect(component.getMealClass('Random')).toBe('');
    expect(component.getMealClass('')).toBe('');
  });

  it('debería alternar y cerrar sidebar', () => {
    component.toggleSidebar();
    expect(component.isSidebarOpen).toBe(true);
    component.closeSidebar();
    expect(component.isSidebarOpen).toBe(false);
  });

  it('debería abrir y cerrar info modal', () => {
    component.openInfoModal({ recipeName: 'Pollo' });
    expect(component.isInfoModalOpen).toBe(true);
    expect(component.selectedRecipe.recipeName).toBe('Pollo');

    component.closeInfoModal();
    expect(component.isInfoModalOpen).toBe(false);
    expect(component.selectedRecipe).toBeNull();
  });

  it('debería crear nuevo plan', () => {
    vi.spyOn(component, 'loadDashboardData');
    component.crearNuevoPlan();
    expect(component.loadDashboardData).toHaveBeenCalled();
  });

  it('debería manejar error al crear nuevo plan', () => {
    vi.spyOn(router, 'navigate');
    vi.spyOn(homeServiceMock, 'createWeeklyPlan').mockReturnValue(throwError(() => ({ status: 404 })));
    component.crearNuevoPlan();
    expect(router.navigate).toHaveBeenCalledWith(['/onboarding']);
  });

  it('debería manejar error general al crear nuevo plan', () => {
    vi.spyOn(window, 'alert');
    vi.spyOn(homeServiceMock, 'createWeeklyPlan').mockReturnValue(throwError(() => ({ status: 501 })));
    component.crearNuevoPlan();
    expect(window.alert).toHaveBeenCalled();
  });

  it('debería redirigir a ver receta', () => {
    vi.spyOn(router, 'navigate');
    component.viewRecipeDetails();
    expect(router.navigate).toHaveBeenCalledWith(['/recipes']);
  });

  it('debería guardar configuracion de dia', () => {
    vi.spyOn(homeServiceMock, 'updateDailyRecipes').mockReturnValue(of({}));
    vi.spyOn(component, 'loadDailyHeader').mockImplementation(() => {}); // Prevent overwrite
    component.dailyHeaders[1] = { idUserFoodPlanDaily: 1, dietName: '', dayName: '', targetKcal: 0, realKcal: 0, targetProtein: 0, realProtein: 0, targetCarbs: 0, realCarbs: 0, targetFat: 0, realFat: 0, mealsPerDay: 5 };
    component.saveDayConfiguration(1, [{ idRecipe: 1, kcal: 100, protein: 10, carbs: 10, fat: 10 }]);
    expect(component.dailyHeaders[1].realKcal).toBe(100);
    expect(component.dailyHeaders[1].realProtein).toBe(10);
  });
});
import { PLATFORM_ID } from '@angular/core';


