import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
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
    idUser: 0, 
    idGoal: 0,
    // 👇 CAMBIO 1: Ahora es un array vacío en lugar de un 0
    idIntolerances: [] as number[], 
    idFoodPlan: 0
  };

  constructor(
    private userService: UserService,
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

  // 👇 CAMBIO 2: Lógica para añadir o quitar el ID de la lista
  selectIntolerance(id: number) {
    const index = this.userData.idIntolerances.indexOf(id);
    if (index > -1) {
      // Si ya estaba seleccionado, lo quitamos del array (desmarcar)
      this.userData.idIntolerances.splice(index, 1);
    } else {
      // Si no estaba, lo añadimos al array (marcar)
      this.userData.idIntolerances.push(id);
    }
  }

  selectFoodPlan(id: number) {
    this.userData.idFoodPlan = id;
  }

  get progressPercentage() {
    return (this.currentStep / this.totalSteps) * 100;
  }

  finishOnboarding() {
  const userId = this.userService.getUserId();
  
  if (!userId) {
    alert('Error: No se ha detectado tu sesión de usuario.');
    this.router.navigate(['/login']);
    return;
  }

  const onboardingData = {
    ...this.userData, 
    idUser: userId    
  };

  this.onboardingService.saveUserData(onboardingData).subscribe({
    next: () => {
      console.log('Datos de salud guardados con éxito.');
      this.router.navigate(['/ingredient-selection']);
    },
    error: (err) => {
      console.error('Error al guardar onboarding:', err);
      alert('Hubo un problema al guardar tus datos.');
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