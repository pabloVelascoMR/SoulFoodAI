import { TestBed } from '@angular/core/testing';
import { IngredientSelectionComponent } from './ingredient-selection.component';
import { IngredientService } from '../../services/ingredient.service';
import { UserIngredientService } from '../../services/user_ingredient.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('IngredientSelectionComponent', () => {
  beforeEach(async () => {
    const ingMock = { getIngredients: () => of([]), searchOpenFoodFacts: () => of([]) };
    const userIngMock = { getSelectedIngredients: () => of([]) };
    const userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [IngredientSelectionComponent],
      providers: [
        provideRouter([]),
        { provide: IngredientService, useValue: ingMock },
        { provide: UserIngredientService, useValue: userIngMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(IngredientSelectionComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
});
