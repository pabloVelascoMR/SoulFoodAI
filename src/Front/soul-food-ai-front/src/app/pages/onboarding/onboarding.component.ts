import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OnboardingService } from '../../services/onboarding.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, FormsModule], 
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
    idUser: 7, 
    idGoal: 0,
    idIntolerance: 0,
    idFoodPlan: 0
  };

  constructor(
    private onboardingService: OnboardingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.onboardingService.getGoals().subscribe(res => this.goals = res);
    this.onboardingService.getIntolerances().subscribe(res => this.intolerances = res);
    this.onboardingService.getFoodPlans().subscribe(res => this.foodPlans = res);
  }

  decrementAge() {
    if (this.userData.age === null) {
      this.userData.age = 28;
    }
    if (this.userData.age > 1) {
      this.userData.age--;
    }
  }

  incrementAge() {
    if (this.userData.age === null) {
      this.userData.age = 28;
    }
    if (this.userData.age < 120) {
      this.userData.age++;
    }
  }

  nextStep() {
    if (this.currentStep === 1) {
      if (!this.userData.gender) {
        alert('Debes seleccionar tu género para continuar.');
        return; 
      }
      if (!this.userData.age || this.userData.age < 1 || this.userData.age > 120) {
        alert('Por favor, introduce una edad real.');
        return; 
      }
    }

    if (this.currentStep === 2) {
      if (!this.userData.height || this.userData.height < 0.5 || this.userData.height > 3.0) {
        alert('ERROR: La altura debe estar en metros. (Ejemplo: 1.75). No uses centímetros.');
        return;
      }
      if (!this.userData.weight || this.userData.weight < 20 || this.userData.weight > 300) {
        alert('ERROR: El peso debe estar entre 20kg y 300kg.');
        return;
      }
      if (!this.userData.mealsPerDay || this.userData.mealsPerDay < 1 || this.userData.mealsPerDay > 5) {
        alert('ERROR: Te recomendamos hacer entre 1 y 5 comidas al día.');
        return;
      }
    }

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
    this.nextStep(); 
  }

  
  selectIntolerance(id: number) {
    
    if (this.userData.idIntolerance === id) {
      this.userData.idIntolerance = 0;
    } else {
      this.userData.idIntolerance = id;
    }
  }

  selectFoodPlan(id: number) {
    this.userData.idFoodPlan = id;
  }

  get progressPercentage() {
    return (this.currentStep / this.totalSteps) * 100;
  }

  finishOnboarding() {
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
  getGoalEmoji(idGoal: number): string {
    const emojis: { [key: number]: string } = {
      1: '🔥', 
      2: '💪', 
      3: '⚖️', 
      4: '🔄', 
      5: '⚡', 
      6: '🥗'  
    };
    return emojis[idGoal] || '🎯';
  }

  getIntoleranceEmoji(idIntolerance: number): string {
  const emojis: { [key: number]: string } = {
    1: '🥛', 
    2: '🌾',
    3: '🌰', 
    4: '🦐', 
    5: '🥚', 
    6: '🌱', 
    7: '🐟', 
    8: '𓇢', 
    9: '🌭',
    10: '🥜', 
    11: '🍯', 
    13: '🍚' 
  };
  return emojis[idIntolerance] || '⚠️';
}

getDietEmoji(idFoodPlan: any): string {
    const id = Number(idFoodPlan);
    const emojis: { [key: number]: string } = {
      1: '🍽️', 
      2: '🥗', 
      3: '🫒', 
      4: '🥑', 
      5: '🧀', 
      6: '🥩', 
      7: '🌱', 
      8: '🥦🥚', 
      9: '🐟', 
      10: '🫐', 
      11: '🌿',
      12: '❤️'
    };

    return emojis[id] || '🎯';
  }
}