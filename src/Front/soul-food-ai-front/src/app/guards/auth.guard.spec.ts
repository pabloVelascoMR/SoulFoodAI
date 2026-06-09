import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { authGuard } from './auth.guard';
import { UserService } from '../services/user.service';

describe('authGuard', () => {
  let routerMock: any;
  let userServiceMock: any;

  beforeEach(() => {
    routerMock = { navigate: function() {} };
    userServiceMock = { getUserId: function() { return 1; } };

    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: UserService, useValue: userServiceMock }
      ]
    });
  });

  it('debería permitir el paso si hay usuario', () => {
    userServiceMock.getUserId = () => 1; 
    const result = TestBed.runInInjectionContext(() => authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot));
    expect(result).toBe(true); // <-- CORREGIDO
  });

  it('debería bloquear y redirigir a / sióno hay usuario', () => {
    userServiceMock.getUserId = () => null; 
    let navigatedTo = '';
    routerMock.navigate = (arr: any) => { navigatedTo = arr[0]; };

    const result = TestBed.runInInjectionContext(() => authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot));
    expect(result).toBe(false); // <-- CORREGIDO
    expect(navigatedTo).toBe('/');
  });
});