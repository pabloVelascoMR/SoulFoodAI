import { TestBed } from '@angular/core/testing';
import { WeekReportComponent } from './week-report.component';
import { WeekReportService } from '../../services/week-report.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('WeekReportComponent', () => {
  beforeEach(async () => {
    const wrMock = { submitReport: () => of({}) };
    const userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [WeekReportComponent],
      providers: [
        provideRouter([]),
        { provide: WeekReportService, useValue: wrMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(WeekReportComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
});