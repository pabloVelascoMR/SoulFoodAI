import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { OnboardingComponent } from './pages/onboarding/onboarding.component';
import { IngredientSelectionComponent } from './pages/ingredient-selection/ingredient-selection.component';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'onboarding', component: OnboardingComponent },
  { path: 'ingredient-selection', component: IngredientSelectionComponent },
  { path: 'home', component: HomeComponent },
];