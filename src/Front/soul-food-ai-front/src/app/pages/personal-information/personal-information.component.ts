import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PersonalInformationService } from '../../services/personal-information.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-personal-information',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './personal-information.component.html',
  styleUrls: ['./personal-information.component.css']
})
export class PersonalInformationComponent implements OnInit {
  userData: any = null;
  editedData: any = null;
  isEditing: boolean = false;
  showSuccessMessage: boolean = false;

  goals: any[] = [];
  intolerances: any[] = [];
  foodPlans: any[] = [];

  activityLevels = [
    { id: 1, title: 'Sedentario', emoji: '😴' },
    { id: 2, title: 'Ligeramente Activo', emoji: '🚶' },
    { id: 3, title: 'Moderadamente Activo', emoji: '🏃' },
    { id: 4, title: 'Muy Activo', emoji: '🏋️' },
    { id: 5, title: 'Extra Activo', emoji: '🏃‍➡️' }
  ];

  constructor(
    private readonly personalInfoService: PersonalInformationService,
    private readonly userService: UserService,
    private readonly router: Router,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const userId = this.userService.getUserId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    this.personalInfoService.getGoals().subscribe((res: any) => this.goals = res?.['$values'] || res || []);
    this.personalInfoService.getIntolerances().subscribe((res: any) => this.intolerances = res?.['$values'] || res || []);
    this.personalInfoService.getFoodPlans().subscribe((res: any) => this.foodPlans = res?.['$values'] || res || []);

    this.loadProfile(userId);
  }

  loadProfile(userId: number) {
    this.personalInfoService.getUserDataById(userId).subscribe({
      next: (res: any) => {
       
        if (!res?.age) {
          this.router.navigate(['/onboarding']);
          return;
        }

        this.userData = res;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("Error al cargar el perfil:", err);
      }
    });
  }

  startEditing(): void {
    this.editedData = structuredClone(this.userData); 
    if (!this.editedData.idIntolerances) {
      this.editedData.idIntolerances = [];
    }
    this.isEditing = true;
  }

  cancelEditing(): void {
    this.isEditing = false;
    this.editedData = null;
  }

  // --- INTOLERANCIAS ---
  toggleIntolerance(id: number): void {
    if (!this.editedData.idIntolerances) this.editedData.idIntolerances = [];
    const index = this.editedData.idIntolerances.indexOf(id);
    if (index > -1) {
      this.editedData.idIntolerances.splice(index, 1);
    } else {
      this.editedData.idIntolerances.push(id);
    }
  }

  hasIntolerance(id: number): boolean {
    return this.editedData?.idIntolerances?.includes(id) || false;
  }

  saveData(): void {
    this.editedData.levelOfActivity = Number(this.editedData.levelOfActivity);
    this.editedData.idGoal = Number(this.editedData.idGoal);
    this.editedData.idFoodPlan = Number(this.editedData.idFoodPlan);

    this.personalInfoService.updateUserData(this.editedData).subscribe({
      next: () => {
        this.userData = { ...this.editedData };
        this.isEditing = false;
        
        this.showSuccessMessage = true;
        this.cdr.detectChanges();

        setTimeout(() => {
          this.showSuccessMessage = false;
          this.cdr.detectChanges();
        }, 5000);
      },
      error: (err) => {
        console.error('Error al guardar', err);
        alert('Hubo un problema al guardar tus datos.');
      }
    });
  }

  getActivityInfo(levelId: number) {
    return this.activityLevels.find(a => a.id == levelId) || this.activityLevels[0];
  }

  getGoalName(idGoal: number): string {
    if (!this.goals || this.goals.length === 0) return '';
    const goal = this.goals.find(g => g.idGoal == idGoal);
    return goal ? goal.goalName : 'No especificado';
  }

  getFoodPlanName(idFoodPlan: number): string {
    if (!this.foodPlans || this.foodPlans.length === 0) return '';
    const plan = this.foodPlans.find(p => p.idFoodPlan == idFoodPlan);
    return plan ? plan.foodPlanName : 'No especificado';
  }

  getIntolerancesNames(): string[] {
    if (!this.userData?.idIntolerances) return ['Ninguna'];
    return this.userData.idIntolerances.map((id: number) => {
      const int = this.intolerances.find(i => i.idIntolerance == id);
      return int ? int.intoleranceName : '';
    }).filter((name: string) => name !== '');
  }

  getGoalEmoji(idGoal: number): string {
    const emojis: { [key: number]: string } = { 1: '🔥', 2: '💪', 3: '⚖️', 4: '🔄', 5: '⚡', 6: '🥗' };
    return emojis[idGoal] || '🎯';
  }

  getDietEmoji(idFoodPlan: any): string {
    const id = Number(idFoodPlan);
    const emojis: { [key: number]: string } = { 1: '🍽️', 2: '🥗', 3: '🫒', 4: '🥑', 5: '🧀', 6: '🥩', 7: '🌱', 8: '🥦🥚', 9: '🐟', 10: '🫐', 11: '🌿', 12: '❤️' };
    return emojis[id] || '🎯';
  }

  logout(): void {
    localStorage.clear();
    sessionStorage.clear();
    this.router.navigate(['/']);
  }

  goHome(): void {
    this.router.navigate(['/home']);
  }
}