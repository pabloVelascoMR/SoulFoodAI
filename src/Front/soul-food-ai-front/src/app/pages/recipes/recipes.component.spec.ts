import { TestBed } from '@angular/core/testing';
import { RecipesComponent } from './recipes.component';
import { RecipesService } from '../../services/recipes.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('RecipesComponent', () => {
  beforeEach(async () => {
    const recipeMock = {
      getUserRecipes: () => of([]),
      getMeals: () => of([]),
      getAllowedIngredients: () => of([])
    };
    const userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [RecipesComponent],
      providers: [
        provideRouter([]),
        { provide: RecipesService, useValue: recipeMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(RecipesComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
});