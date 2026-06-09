import { TestBed } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { HomeService } from '../../services/home.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('HomeComponent', () => {
  beforeEach(async () => {
    const homeMock = {
      getWeeklyHeader: () => of({}),
      getActiveWeekCalendar: () => of({ days: [] }),
      getDailyHeader: () => of({}),
      getRecipesForUser: () => of([])
    };
    const userMock = { getUserId: () => 1, logout: () => {} };

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        provideRouter([]),
        { provide: HomeService, useValue: homeMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(HomeComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
});