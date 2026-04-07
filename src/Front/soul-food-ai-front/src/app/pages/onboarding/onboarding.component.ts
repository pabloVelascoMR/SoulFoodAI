import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OnboardingService } from '../../services/onboarding.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule], // Importante para que funcione la pantalla
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.css']
})
export class OnboardingComponent implements OnInit {
  currentStep = 1;
  totalSteps = 5;

  goals: any[] = [];
  intolerances: any[] = [];
  foodPlans: any[] = [];

  userData = {
    gender: '',
    age: null as number | null,
    height: null as number | null,
    weight: null as number | null,
    mealsPerDay: 3,
    idUser: 1, 
    idGoal: null as number | null,
    idIntolerance: null as number | null,
    idFoodPlan: null as number | null
  };

  constructor(
    private onboardingService: OnboardingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Al abrir la pantalla, mandamos al mensajero a por las opciones
    this.onboardingService.getGoals().subscribe(res => this.goals = res);
    this.onboardingService.getIntolerances().subscribe(res => this.intolerances = res);
    this.onboardingService.getFoodPlans().subscribe(res => this.foodPlans = res);
  }

  nextStep() {
    if (this.currentStep < this.totalSteps) {
      this.currentStep++;
    }
  }

  prevStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  
  selectGender(gender: string) {
    this.userData.gender = gender;
  }

  selectGoal(id: number) {
    this.userData.idGoal = id;
    setTimeout(() => this.nextStep(), 350); 
  }

  selectIntolerance(id: number) {
    this.userData.idIntolerance = id;
    setTimeout(() => this.nextStep(), 350);
  }

  selectFoodPlan(id: number) {
    this.userData.idFoodPlan = id;
  }

  get progressPercentage() {
    return (this.currentStep / this.totalSteps) * 100;
  }

  finishOnboarding() {
    // Mandamos el mensajero a tu API
    this.onboardingService.saveUserData(this.userData).subscribe({
      next: () => {
        alert('¡Plan guardado con éxito!');
      },
      error: (err) => {
        console.error('Error guardando los datos', err);
        alert('Hubo un error al guardar.');
      }
    });
  }
}