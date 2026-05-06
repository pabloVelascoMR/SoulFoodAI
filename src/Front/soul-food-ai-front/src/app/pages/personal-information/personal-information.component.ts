import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PersonalInformationService } from '../../services/personal-information.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-personal-information',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './personal-information.component.html',
  styleUrls: ['./personal-information.component.css']
})
export class PersonalInformationComponent implements OnInit {
  userData: any = null;
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
    private personalInfoService: PersonalInformationService,
    private userService: UserService,
    private router: Router,
    private cdr: ChangeDetectorRef // <--- CLAVE PARA QUE SE QUITE EL "CARGANDO..."
  ) {}

  ngOnInit(): void {
    const userId = this.userService.getUserId();
    console.log("-> 1. Iniciando carga para el usuario:", userId);
    
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }

    // Cargamos los catálogos
    this.personalInfoService.getGoals().subscribe((res: any) => {
      this.goals = res?.['$values'] || res || [];
      console.log("-> 2. Objetivos cargados");
    });

    this.personalInfoService.getIntolerances().subscribe((res: any) => {
      this.intolerances = res?.['$values'] || res || [];
    });

    this.personalInfoService.getFoodPlans().subscribe((res: any) => {
      this.foodPlans = res?.['$values'] || res || [];
    });

    // Búsqueda del perfil
    this.personalInfoService.getAllUserDatas().subscribe({
      next: (res: any) => {
        console.log("-> 3. Respuesta cruda UserData recibida:", res);

        if (!res) {
          this.router.navigate(['/onboarding']);
          return;
        }

        let dataArray: any[] = [];
        if (Array.isArray(res)) {
          dataArray = res;
        } else if (res['$values']) { 
          dataArray = res['$values'];
        } else if (res['data']) {
          dataArray = res['data'];
        }

        console.log("-> 4. Array de perfiles listo. Total:", dataArray.length);

        // Búsqueda super flexible que cubre cualquier forma en la que C# envíe el ID
        const userProfile = dataArray.find((d: any) => 
          d.idUser == userId || 
          d.userId == userId || 
          d.idUserData == userId || 
          d.IdUser == userId
        );

        console.log("-> 5. Perfil encontrado:", userProfile);

        if (userProfile) {
          this.userData = userProfile;
          this.cdr.detectChanges(); // <--- OBLIGAMOS A ANGULAR A PINTAR EL HTML
        } else {
          alert('Aún no tienes medidas guardadas. ¡Vamos al onboarding a crearlas!');
          this.router.navigate(['/onboarding']);
        }
      },
      error: (err) => {
        console.error("-> ERROR FATAL al obtener los UserData:", err);
      }
    });
  }

  editData(): void {
    this.router.navigate(['/onboarding']);
  }

  getActivityInfo() {
    if (!this.userData) return this.activityLevels[0];
    return this.activityLevels.find(a => a.id == this.userData.levelOfActivity) || this.activityLevels[0];
  }

  getGoalName(): string {
    if (!this.userData || !Array.isArray(this.goals)) return 'No especificado';
    const goal = this.goals.find(g => g.idGoal == this.userData.idGoal);
    return goal ? goal.goalName : 'No especificado';
  }

  getFoodPlanName(): string {
    if (!this.userData || !Array.isArray(this.foodPlans)) return 'No especificado';
    const plan = this.foodPlans.find(p => p.idFoodPlan == this.userData.idFoodPlan);
    return plan ? plan.foodPlanName : 'No especificado';
  }

  getIntolerancesNames(): string[] {
    if (!this.userData || !this.userData.idIntolerances || !Array.isArray(this.intolerances)) {
      return ['Ninguna'];
    }
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
}