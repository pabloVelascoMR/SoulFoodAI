import { vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { PersonalInformationComponent } from './personal-information.component';
import { PersonalInformationService } from '../../services/personal-information.service';
import { UserService } from '../../services/user.service';
import { provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';

describe('PersonalInformationComponent', () => {
  let component: PersonalInformationComponent;
  let piMock: any;
  let userMock: any;
  let router: any;

  beforeEach(async () => {
    piMock = {
      getGoals: () => of([{ idGoal: 1, goalName: 'Goal 1' }]),
      getIntolerances: () => of([{ idIntolerance: 1, intoleranceName: 'Intolerance 1' }]),
      getFoodPlans: () => of([{ idFoodPlan: 1, foodPlanName: 'Plan 1' }]),
      getUserDataById: () => of({ age: 25, idIntolerances: [1], levelOfActivity: 2, idGoal: 1, idFoodPlan: 1 }),
      updateUserData: () => of({})
    };
    userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [PersonalInformationComponent],
      providers: [
        provideRouter([]),
        { provide: PersonalInformationService, useValue: piMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(PersonalInformationComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');
  });

  it('debería inicializar el componente y cargar el perfil', () => {
    component.ngOnInit();
    expect(component.userData).toBeDefined();
    expect(component.goals.length).toBeGreaterThan(0);
    expect(component.intolerances.length).toBeGreaterThan(0);
  });

  it('debería redirigir a login si no hay usuario', () => {
    vi.spyOn(userMock, 'getUserId').mockReturnValue(null);
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería iniciar y cancelar edición', () => {
    component.userData = { idIntolerances: [] };
    component.startEditing();
    expect(component.isEditing).toBe(true);
    expect(component.editedData).toBeDefined();

    component.cancelEditing();
    expect(component.isEditing).toBe(false);
    expect(component.editedData).toBeNull();
  });

  it('debería alternar intolerancias', () => {
    component.editedData = { idIntolerances: [] };
    component.toggleIntolerance(1);
    expect(component.editedData.idIntolerances).toContain(1);
    component.toggleIntolerance(1);
    expect(component.editedData.idIntolerances).not.toContain(1);
  });

  it('debería comprobar si tiene intolerancia', () => {
    component.editedData = { idIntolerances: [2] };
    expect(component.hasIntolerance(2)).toBe(true);
    expect(component.hasIntolerance(3)).toBe(false);
  });

  it('debería guardar datos', () => {
    component.editedData = { levelOfActivity: '2', idGoal: '1', idFoodPlan: '1' };
    component.saveData();
    expect(component.isEditing).toBe(false);
    expect(component.showSuccessMessage).toBe(true);
  });

  it('debería obtener info de actividad, metas y dieta', () => {
    component.goals = [{ idGoal: 1, goalName: 'Goal 1' }];
    component.foodPlans = [{ idFoodPlan: 1, foodPlanName: 'Plan 1' }];
    
    expect(component.getActivityInfo(1).title).toBe('Sedentario');
    expect(component.getGoalName(1)).toBe('Goal 1');
    expect(component.getFoodPlanName(1)).toBe('Plan 1');
    expect(component.getGoalEmoji(1)).toBe('🔥');
    expect(component.getDietEmoji(1)).toBe('🍽️');
  });

  it('debería obtener nombres de intolerancias', () => {
    component.userData = { idIntolerances: [1] };
    component.intolerances = [{ idIntolerance: 1, intoleranceName: 'Gluten' }];
    expect(component.getIntolerancesNames()).toContain('Gluten');
  });

  it('debería cerrar sesión e ir a home', () => {
    component.logout();
    expect(router.navigate).toHaveBeenCalledWith(['/']);
    component.goHome();
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });
});


