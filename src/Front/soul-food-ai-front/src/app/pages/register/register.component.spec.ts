import { TestBed } from '@angular/core/testing';
import { RegisterComponent } from './register.component';
import { UserService } from '../../services/user.service';
import {  Router } from '@angular/router';
import { of } from 'rxjs';

describe('RegisterComponent', () => {
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    userServiceMock = {
      register: () => of({}) 
    };
    routerMock = {
      navigate: function() {}
    };

    await TestBed.configureTestingModule({
      imports: [RegisterComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(RegisterComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('debería fallar si faltan campos', () => {
    const fixture = TestBed.createComponent(RegisterComponent);
    const component = fixture.componentInstance;
    component.register();
    expect(component.errorMessage).toBe('Todos los campos son obligatorios.');
  });

  it('debería fallar sióno se aceptan los términos', () => {
    const fixture = TestBed.createComponent(RegisterComponent);
    const component = fixture.componentInstance;
    
    component.username = 'Pablo';
    component.email = 'test@test.com';
    component.password = 'Pass1234';
    component.confirmPassword = 'Pass1234';
    component.termsAccepted = false;
    
    component.register();
    expect(component.errorMessage).toBe('Debes aceptar la Política de Privacidad y el tratamiento de datos.');
  });

  it('debería navegar a /onboarding si el registro es OK', () => {
    const fixture = TestBed.createComponent(RegisterComponent);
    const component = fixture.componentInstance;
    
    let navigatedTo = '';
    routerMock.navigate = (arr: any) => { navigatedTo = arr[0]; };

    component.username = 'Pablo';
    component.email = 'test@test.com';
    component.password = 'Pass1234';
    component.confirmPassword = 'Pass1234';
    component.termsAccepted = true;
    
    component.register();
    expect(navigatedTo).toBe('/onboarding');
  });
});