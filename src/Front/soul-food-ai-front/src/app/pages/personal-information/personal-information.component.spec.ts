import { TestBed } from '@angular/core/testing';
import { PersonalInformationComponent } from './personal-information.component';
import { PersonalInformationService } from '../../services/personal-information.service';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

describe('PersonalInformationComponent', () => {
  beforeEach(async () => {
    const piMock = {
      getUserData: () => of({}),
      updateUserData: () => of({})
    };
    const userMock = { getUserId: () => 1 };

    await TestBed.configureTestingModule({
      imports: [PersonalInformationComponent],
      providers: [
        provideRouter([]),
        { provide: PersonalInformationService, useValue: piMock },
        { provide: UserService, useValue: userMock }
      ]
    }).compileComponents();
  });

  it('debería crearse el componente', () => {
    const fixture = TestBed.createComponent(PersonalInformationComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });
});