import { TestBed } from '@angular/core/testing';
import { FoodplanHistoryComponent } from './foodplan-history.component';
import { FoodplanHistoryService } from '../../services/foodplan-history.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('FoodplanHistoryComponent', () => {
  beforeEach(async () => {
    const fhMock = { getFoodPlanHistory: () => of([]) };
    const userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [FoodplanHistoryComponent],
      providers: [
        provideRouter([]),
        { provide: FoodplanHistoryService, useValue: fhMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(FoodplanHistoryComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
});