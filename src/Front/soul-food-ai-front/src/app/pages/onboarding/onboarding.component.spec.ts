import { vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { OnboardingComponent } from './onboarding.component';
import { UserService } from '../../services/user.service';
import { OnboardingService } from '../../services/onboarding.service';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('OnboardingComponent', () => {
  let component: OnboardingComponent;
  let userServiceMock: any;
  let onboardingServiceMock: any;
  let router: any;
  let fixture: any;

  beforeEach(async () => {
    userServiceMock = { getUserId: () => 1 };
    onboardingServiceMock = {
      getGoals: () => of([]),
      getIntolerances: () => of([]),
      getFoodPlans: () => of([]),
      saveUserData: () => of({})
    };

    await TestBed.configureTestingModule({
      imports: [OnboardingComponent],
      providers: [
        provideRouter([]),
        { provide: UserService, useValue: userServiceMock },
        { provide: OnboardingService, useValue: onboardingServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(OnboardingComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');
  });

  it('debería inicializar los catálogos al cargar', () => {
    component.ngOnInit();
    expect(component.goals).toBeDefined();
    expect(component.intolerances).toBeDefined();
    expect(component.foodPlans).toBeDefined();
  });

  it('debería manejar progreso e incrementar step', () => {
    component.currentStep = 1;
    component.totalSteps = 6;
    expect(component.progressPercentage).toBe((1/6)*100);

    vi.spyOn(component, 'validateStep1').mockReturnValue(true);
    component.nextStep();
    expect(component.currentStep).toBe(2);

    component.prevStep();
    expect(component.currentStep).toBe(1);
  });

  it('debería fallar validacion 1 si faltan datos', () => {
    vi.spyOn(globalThis, 'alert');
    component.userData.gender = '';
    expect(component.validateStep1()).toBe(false);

    component.userData.gender = 'M';
    component.userData.age = null;
    expect(component.validateStep1()).toBe(false);
  });

  it('debería fallar validacion 2 por altura o peso', () => {
    vi.spyOn(globalThis, 'alert');
    component.userData.height = 0;
    expect(component.validateStep2()).toBe(false);

    component.userData.height = 1.8;
    component.userData.weight = 10;
    expect(component.validateStep2()).toBe(false);

    component.userData.weight = 80;
    component.userData.mealsPerDay = 10;
    expect(component.validateStep2()).toBe(false);
  });

  it('debería fallar validacion 3 por medidas locas', () => {
    vi.spyOn(globalThis, 'alert');
    component.userData.chestMeasure = 10; // min 30
    expect(component.validateStep3()).toBe(false);

    component.userData.chestMeasure = 300; // max 200
    expect(component.validateStep3()).toBe(false);

    component.userData.chestMeasure = 100;
    component.userData.waistMeasure = null; // null is valid
    expect(component.validateStep3()).toBe(true);
  });

  it('debería pasar validaciones correctamente', () => {
    component.userData.gender = 'M';
    component.userData.age = 25;
    expect(component.validateStep1()).toBe(true);

    component.userData.height = 1.8;
    component.userData.weight = 80;
    component.userData.mealsPerDay = 3;
    expect(component.validateStep2()).toBe(true);

    component.userData.chestMeasure = 100;
    expect(component.validateStep3()).toBe(true);
  });

  it('debería incrementar y decrementar edad', () => {
    component.userData.age = null;
    component.incrementAge();
    expect(component.userData.age as any).toBe(19);

    component.incrementAge();
    expect(component.userData.age as any).toBe(20);

    component.userData.age = 120;
    component.incrementAge();
    expect(component.userData.age as any).toBe(120); // Not incremented

    component.userData.age = null;
    component.decrementAge();
    expect(component.userData.age as any).toBe(17);

    component.decrementAge();
    expect(component.userData.age as any).toBe(16);

    component.userData.age = 1;
    component.decrementAge();
    expect(component.userData.age as any).toBe(1); // Not decremented
  });

  it('debería detener nextStep si falla la validación', () => {
    component.currentStep = 1;
    vi.spyOn(component, 'validateStep1').mockReturnValue(false);
    component.nextStep();
    expect(component.currentStep).toBe(1); // Blocked

    component.currentStep = 2;
    vi.spyOn(component, 'validateStep2').mockReturnValue(false);
    component.nextStep();
    expect(component.currentStep).toBe(2); // Blocked

    component.currentStep = 3;
    vi.spyOn(component, 'validateStep3').mockReturnValue(false);
    component.nextStep();
    expect(component.currentStep).toBe(3); // Blocked
  });

  it('debería seleccionar y de-seleccionar intolerancias', () => {
    component.userData.idIntolerances = [];
    component.selectIntolerance(1);
    expect(component.userData.idIntolerances).toContain(1);

    component.selectIntolerance(1);
    expect(component.userData.idIntolerances).not.toContain(1);
  });

  it('debería seleccionar genero, meta, plan y actividad', () => {
    vi.spyOn(component, 'nextStep');
    component.selectGender('F');
    expect(component.userData.gender).toBe('F');

    component.selectGoal(1);
    expect(component.userData.idGoal).toBe(1);
    expect(component.nextStep).toHaveBeenCalled();

    component.selectFoodPlan(1);
    expect(component.userData.idFoodPlan).toBe(1);

    component.selectActivityLevel(2);
    expect(component.userData.levelOfActivity).toBe(2);
    expect(component.currentActivity.title).toBe('Ligeramente Activo');
  });

  it('debería obtener emojis', () => {
    expect(component.getGoalEmoji(1)).toBe('🔥');
    expect(component.getIntoleranceEmoji(1)).toBe('🥛');
    expect(component.getDietEmoji(1)).toBe('🍽️');

    expect(component.getGoalEmoji(999)).toBe('🎯');
    expect(component.getIntoleranceEmoji(999)).toBe('⚠️');
    expect(component.getDietEmoji(999)).toBe('🎯');
  });

  it('debería redirigir a login al finalizar sin usuario', () => {
    vi.spyOn(globalThis, 'alert');
    vi.spyOn(userServiceMock, 'getUserId').mockReturnValue(null);
    component.finishOnboarding();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería finalizar onboarding exitosamente', () => {
    component.finishOnboarding();
    expect(router.navigate).toHaveBeenCalledWith(['/ingredient-selection']);
  });

  it('debería manejar error al finalizar', () => {
    vi.spyOn(globalThis, 'alert');
    vi.spyOn(onboardingServiceMock, 'saveUserData').mockReturnValue(throwError(() => new Error('Simulated error')));
    component.finishOnboarding();
    expect(globalThis.alert).toHaveBeenCalled();
  });

  it('debería renderizar la vista para todos los pasos (template coverage)', () => {
    component.goals = [{ idGoal: 1, goalName: 'Goal 1' }];
    component.intolerances = [{ idIntolerance: 1, intoleranceName: 'Milk' }];
    component.foodPlans = [{ idFoodPlan: 1, dietName: 'Vegan' }];
    
    vi.spyOn(component, 'progressPercentage', 'get').mockReturnValue(50);

    fixture.detectChanges(false); // init (step 1)
    
    vi.spyOn(component, 'validateStep1').mockReturnValue(true);
    component.nextStep();
    fixture.detectChanges(false); // step 2
    
    vi.spyOn(component, 'validateStep2').mockReturnValue(true);
    component.nextStep();
    fixture.detectChanges(false); // step 3
    
    vi.spyOn(component, 'validateStep3').mockReturnValue(true);
    component.nextStep();
    fixture.detectChanges(false); // step 4
    
    component.nextStep();
    fixture.detectChanges(false); // step 5
    
    component.nextStep();
    fixture.detectChanges(false); // step 6
    
    expect(component.currentStep).toBe(6);
  });
});


