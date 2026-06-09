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
    private readonly userService: UserService,
    private readonly onboardingService: OnboardingService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.onboardingService.getGoals().subscribe(res => this.goals = res);
    this.onboardingService.getIntolerances().subscribe(res => this.intolerances = res);
    this.onboardingService.getFoodPlans().subscribe(res => this.foodPlans = res);
  }

  get currentActivity() {
    return this.activityLevels.find(a => a.id == this.userData.levelOfActivity) || this.activityLevels[0];
  }

  decrementAge() {
    if (this.userData.age === null) {
      this.userData.age = 17;
    } else if (this.userData.age > 1) {
      this.userData.age--;
    }
  }

  incrementAge() {
    if (this.userData.age === null) {
      this.userData.age = 19;
    } else if (this.userData.age < 120) {
      this.userData.age++;
    }
  }

  validateStep1(): boolean {
    if (!this.userData.gender) {
      alert('Debes seleccionar tu género para continuar.');
      return false; 
    }
    if (!this.userData.age || this.userData.age < 1 || this.userData.age > 120) {
      alert('Por favor, introduce una edad real.');
      return false; 
    }
    return true;
  }

  validateStep2(): boolean {
    if (!this.userData.height || this.userData.height < 0.5 || this.userData.height > 3) {
      alert('ERROR: La altura debe estar en metros. (Ejemplo: 1.75). No uses centímetros.');
      return false;
    }
    if (!this.userData.weight || this.userData.weight < 20 || this.userData.weight > 300) {
      alert('ERROR: El peso debe estar entre 20kg y 300kg.');
      return false;
    }
    if (!this.userData.mealsPerDay || this.userData.mealsPerDay < 1 || this.userData.mealsPerDay > 5) {
      alert('ERROR: Te recomendamos hacer entre 1 y 5 comidas al día.');
      return false;
    }
    return true;
  }

  validateStep3(): boolean {
    const limits = [
      { val: this.userData.chestMeasure, min: 30, max: 200, name: 'el pecho' },
      { val: this.userData.waistMeasure, min: 30, max: 200, name: 'la cintura' },
      { val: this.userData.hipMeasure, min: 30, max: 200, name: 'la cadera' },
      { val: this.userData.leftBicepMeasure, min: 10, max: 100, name: 'el bíceps izquierdo' },
      { val: this.userData.rightBicepMeasure, min: 10, max: 100, name: 'el bíceps derecho' },
      { val: this.userData.leftCuadricepsMeasure, min: 20, max: 150, name: 'el cuádriceps izquierdo' },
      { val: this.userData.rightCuadricepsMeasure, min: 20, max: 150, name: 'el cuádriceps derecho' }
    ];

    for (const measure of limits) {
      if (measure.val !== null && (measure.val < measure.min || measure.val > measure.max)) {
        alert(`ERROR: Si introduces ${measure.name}, debe estar entre ${measure.min} y ${measure.max} cm.`);
        return false;
      }
    }
    return true;
  }

  nextStep() {
    if (this.currentStep === 1 && !this.validateStep1()) return;
    if (this.currentStep === 2 && !this.validateStep2()) return;
    if (this.currentStep === 3 && !this.validateStep3()) return;

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
        this.router.navigate(['/ingredient-selection']);
      },
      error: (err) => {
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