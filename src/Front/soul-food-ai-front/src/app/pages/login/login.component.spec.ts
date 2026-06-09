import { TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { UserService } from '../../services/user.service';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

describe('LoginComponent', () => {
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    userServiceMock = {
      login: () => of({})
    };
    routerMock = {
      navigate: function() {}
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('debería mostrar error si faltan campos', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;
    
    component.email = '';
    component.password = '';
    component.login();
    
    expect(component.errorMessage).toBe('Por favor, rellena todos los campos.');
  });

  it('debería navegar a /home si el login es exitoso', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;
    
    let navigatedTo = '';
    routerMock.navigate = (arr: any) => { navigatedTo = arr[0]; };

    component.email = 'test@test.com';
    component.password = '1234';
    component.login();
    
    expect(navigatedTo).toBe('/home');
  });

  it('debería mostrar error 401 si falla el login', () => {
    const fixture = TestBed.createComponent(LoginComponent);
    const component = fixture.componentInstance;
    
    userServiceMock.login = () => throwError(() => ({ status: 401 }));

    component.email = 'test@test.com';
    component.password = 'mal';
    component.login();

    expect(component.errorMessage).toBe('Email o contraseña incorrectos.');
    expect(component.isSubmitting).toBe(false);
  });
});