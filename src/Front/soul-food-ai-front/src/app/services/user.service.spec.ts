import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UserService } from './user.service';
import { PLATFORM_ID } from '@angular/core';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        UserService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: PLATFORM_ID, useValue: 'browser' }
      ]
    });
    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería hacer login', () => {
    service.login({ email: 'test@test.com', password: '123' }).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/Login'));
    expect(req.request.method).toBe('POST');
    req.flush({ token: 'abc', idUser: 1 });
  });

  it('debería registrar un usuario', () => {
    service.register({ userName: 'Pablo' }).subscribe();
    const req = httpMock.expectOne(req => req.url.includes('/AddUser'));
    expect(req.request.method).toBe('POST');
    req.flush({ token: 'abc', idUser: 1 });
  });
});