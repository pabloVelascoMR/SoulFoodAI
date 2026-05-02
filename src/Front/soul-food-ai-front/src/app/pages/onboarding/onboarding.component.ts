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
  totalSteps = 6;

  goals: any[] = [];
  intolerances: any[] = [];
  foodPlans: any[] = [];

  userData = {
    gender: '',
    age: null as number | null,
    height: null as number | null,
    weight: null as number | null,
    mealsPerDay: 3,
    levelOfActivity: 1,
    chestMeasure: null as number | null,
    waistMeasure: null as number | null,
    hipMeasure: null as number | null,
    leftBicepMeasure: null as number | null,
    rightBicepMeasure: null as number | null,
    leftCuadricepsMeasure: null as number | null,
    rightCuadricepsMeasure: null as number | null,
    idUser: 0, 
    idGoal: 0,
    idIntolerances: [] as number[], 
    idFoodPlan: 0
  };

  activityLevels = [
    { id: 1, title: 'Sedentario', desc: 'Poco o ningún ejercicio, trabajo de oficina.' },
    { id: 2, title: 'Ligeramente Activo', desc: 'Ejercicio ligero o deportes 1-3 días a la semana.' },
    { id: 3, title: 'Moderadamente Activo', desc: 'Ejercicio moderado o deportes 3-5 días a la semana.' },
    { id: 4, title: 'Muy Activo', desc: 'Ejercicio duro o deportes 6-7 días a la semana.' },
    { id: 5, title: 'Extra Activo', desc: 'Ejercicio muy duro o trabajo físico.' }
  ];

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

    if (this.currentStep === 3) {
      // Validamos los opcionales SOLO si el usuario ha escrito algo
      if (this.userData.chestMeasure !== null && (this.userData.chestMeasure < 30 || this.userData.chestMeasure > 200)) {
        alert('ERROR: Si introduces el pecho, debe estar entre 30 y 200 cm.'); return;
      }
      if (this.userData.waistMeasure !== null && (this.userData.waistMeasure < 30 || this.userData.waistMeasure > 200)) {
        alert('ERROR: Si introduces la cintura, debe estar entre 30 y 200 cm.'); return;
      }
      if (this.userData.hipMeasure !== null && (this.userData.hipMeasure < 30 || this.userData.hipMeasure > 200)) {
        alert('ERROR: Si introduces la cadera, debe estar entre 30 y 200 cm.'); return;
      }
      if (this.userData.leftBicepMeasure !== null && (this.userData.leftBicepMeasure < 10 || this.userData.leftBicepMeasure > 100)) {
        alert('ERROR: Si introduces el bíceps izquierdo, debe estar entre 10 y 100 cm.'); return;
      }
      if (this.userData.rightBicepMeasure !== null && (this.userData.rightBicepMeasure < 10 || this.userData.rightBicepMeasure > 100)) {
        alert('ERROR: Si introduces el bíceps derecho, debe estar entre 10 y 100 cm.'); return;
      }
      if (this.userData.leftCuadricepsMeasure !== null && (this.userData.leftCuadricepsMeasure < 20 || this.userData.leftCuadricepsMeasure > 150)) {
        alert('ERROR: Si introduces el cuádriceps izquierdo, debe estar entre 20 y 150 cm.'); return;
      }
      if (this.userData.rightCuadricepsMeasure !== null && (this.userData.rightCuadricepsMeasure < 20 || this.userData.rightCuadricepsMeasure > 150)) {
        alert('ERROR: Si introduces el cuádriceps derecho, debe estar entre 20 y 150 cm.'); return;
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

  selectActivityLevel(level: number) {
    this.userData.levelOfActivity = level;
  }

  selectIntolerance(id: number) {
    const index = this.userData.idIntolerances.indexOf(id);
    if (index > -1) {
      this.userData.idIntolerances.splice(index, 1);
    } else {
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