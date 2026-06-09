import { TestBed } from '@angular/core/testing';
import { OnboardingComponent } from './onboarding.component';
import { UserService } from '../../services/user.service';
import { OnboardingService } from '../../services/onboarding.service';
import { provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';

describe('OnboardingComponent', () => {
  let userServiceMock: any;
  let onboardingServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    userServiceMock = {
      getUserId: () => 1
    };
    onboardingServiceMock = {
      getGoals: () => of([{ idGoal: 1, name: 'Bajar peso' }]),
      getIntolerances: () => of([{ idIntolerance: 1, name: 'Lactosa' }]),
      getFoodPlans: () => of([{ idFoodPlan: 1, name: 'Mediterránea' }]),
      saveUserData: () => of({})
    };
    routerMock = {
      navigate: function() {}
    };

    await TestBed.configureTestingModule({
      imports: [OnboardingComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: OnboardingService, useValue: onboardingServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  it('debería inicializar los catálogos al cargar', () => {
    const fixture = TestBed.createComponent(OnboardingComponent);
    const component = fixture.componentInstance;
    
    fixture.detectChanges();
    
    expect(component).toBeTruthy();
    expect(component.goals.length).toBe(1);
    expect(component.intolerances.length).toBe(1);
  });

  it('debería avanzar de paso si los datos del paso 1 son válidos', () => {
    const fixture = TestBed.createComponent(OnboardingComponent);
    const component = fixture.componentInstance;
    
    component.currentStep = 1;
    component.userData.gender = 'Hombre';
    component.userData.age = 25;
    
    component.nextStep();
    expect(component.currentStep).toBe(2);
  });

  it('debería navegar a ingredient-selection al terminar', () => {
    const fixture = TestBed.createComponent(OnboardingComponent);
    const component = fixture.componentInstance;
    
    let navigatedTo = '';
    routerMock.navigate = (arr: any) => { navigatedTo = arr[0]; };

    component.finishOnboarding();
    expect(navigatedTo).toBe('/ingredient-selection');
  });
});