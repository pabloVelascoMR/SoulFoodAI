import { Routes } from '@angular/router';

import { LandingPageComponent } from './pages/landing-page/landing-page.component'; 
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { OnboardingComponent } from './pages/onboarding/onboarding.component';
import { IngredientSelectionComponent } from './pages/ingredient-selection/ingredient-selection.component';
import { HomeComponent } from './pages/home/home.component';
import { RecipesComponent } from './pages/recipes/recipes.component';
import { PersonalInformationComponent } from './pages/personal-information/personal-information.component';
import { WeekReportComponent } from './pages/week-report/week-report.component';
import { FoodplanHistoryComponent } from './pages/foodplan-history/foodplan-history.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
 
  { path: '', component: LandingPageComponent }, 
  { path: 'login', component: LoginComponent },  
  { path: 'register', component: RegisterComponent },
  { path: 'onboarding', component: OnboardingComponent, canActivate: [authGuard] },
  { path: 'ingredient-selection', component: IngredientSelectionComponent, canActivate: [authGuard] },
  { path: 'home', component: HomeComponent, canActivate: [authGuard] },
  { path: 'recipes', component: RecipesComponent, canActivate: [authGuard] },
  { path: 'personal-information', component: PersonalInformationComponent, canActivate: [authGuard] },
  { path: 'week-report', component: WeekReportComponent, canActivate: [authGuard] },
  { path: 'foodplan-historial', component: FoodplanHistoryComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' } 
];