import { TestBed } from '@angular/core/testing';
import { WeekReportComponent } from './week-report.component';
import { WeekReportService } from '../../services/week-report.service';
import { UserService } from '../../services/user.service';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';

describe('WeekReportComponent', () => {
  let component: WeekReportComponent;
  let reportServiceMock: any;
  let userServiceMock: any;
  let router: any;

  beforeEach(async () => {
    reportServiceMock = { submitReport: () => of({ aiAnalysis: 'Test Feedback' }) };
    userServiceMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [WeekReportComponent],
      providers: [
        provideRouter([]),
        { provide: WeekReportService, useValue: reportServiceMock },
        { provide: UserService, useValue: userServiceMock }
      ]
    }).compileComponents();

    const fixture = TestBed.createComponent(WeekReportComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate');
  });

  it('debería inicializar el componente y obtener usuario', () => {
    component.ngOnInit();
    expect(component.report.idUser).toBe(1);
  });

  it('debería redirigir a login si no hay usuario', () => {
    vi.spyOn(userServiceMock, 'getUserId').mockReturnValue(null);
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería ir a home', () => {
    component.goHome();
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('debería alternar medidas', () => {
    expect(component.showOptionalMeasures).toBe(false);
    component.toggleMeasures();
    expect(component.showOptionalMeasures).toBe(true);
    component.toggleMeasures();
    expect(component.showOptionalMeasures).toBe(false);
  });

  it('debería cambiar entre pasos', () => {
    component.goToStep2();
    expect(component.step).toBe(2);
    component.backToStep1();
    expect(component.step).toBe(1);
  });

  it('debería prevenir generar plan si está cargando', () => {
    component.isLoading = true;
    vi.spyOn(reportServiceMock, 'submitReport');
    component.generatePlan(true);
    expect(reportServiceMock.submitReport).not.toHaveBeenCalled();
  });

  it('debería generar plan con AI y recibir feedback', () => {
    component.showOptionalMeasures = true;
    component.report.newWeight = 80 as any;
    component.report.newMeasures.chestMeasure = 100;
    
    vi.spyOn(reportServiceMock, 'submitReport').mockReturnValue(of({ aiAnalysis: 'Todo bien' }));
    
    component.generatePlan(true);
    expect(component.isLoading).toBe(false);
    expect(component.step).toBe(3);
    expect(component.aiFeedback).toBe('Todo bien');
  });

  it('debería manejar error al generar plan', () => {
    vi.spyOn(reportServiceMock, 'submitReport').mockReturnValue(throwError(() => new Error('Error')));
    component.generatePlan(false);
    expect(component.isLoading).toBe(false);
    expect(component.errorMessage).toContain('error');
  });
});
