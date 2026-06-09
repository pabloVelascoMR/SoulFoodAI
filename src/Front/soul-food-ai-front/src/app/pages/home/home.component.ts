import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem, copyArrayItem } from '@angular/cdk/drag-drop';
import { Router, RouterModule } from '@angular/router'; 
import { UserService } from '../../services/user.service';
import { timeout } from 'rxjs/operators';
import { 
  HomeService, 
  WeekCalendarDto, 
  WeeklyHeaderDto, 
  DailyHeaderDto,
} from '../../services/home.service';

@Component({
  selector: 'app-home',
  standalone: true, 
  imports: [CommonModule, DragDropModule, RouterModule], 
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  userId: number = 0; 
  hasActivePlan: boolean = false;
  loading: boolean = true;
  weeklyHeader: WeeklyHeaderDto | null = null;
  calendar: WeekCalendarDto | null = null;
  availableRecipes: any[] = [];
  connectedLists: string[] = ['recipe-catalog'];
  dailyHeaders: { [key: number]: DailyHeaderDto } = {};
  isSidebarOpen: boolean = false;
  selectedDayId: number | null = null;
  isAdjustingMacros: boolean = false;
  aiMessage: string = '';
  aiMessageType: 'success' | 'error' | '' = '';
  isInfoModalOpen: boolean = false;
  selectedRecipe: any = null;

  constructor(
    private readonly homeService: HomeService,
    private readonly userService: UserService,
    private readonly router: Router,
    @Inject(PLATFORM_ID) private readonly platformId: Object,
    private readonly cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const id = this.userService.getUserId();
      if (!id) {
        this.router.navigate(['/login']);
        return;
      }
      this.userId = id;
      this.loadDashboardData();
    }
  }

  selectDay(idDaily: number) {
    this.selectedDayId = idDaily;
  }

  clearSelectedDay() {
    this.selectedDayId = null;
  }

  getDayName(idDaily: number): string {
    const day = this.calendar?.days.find(d => d.idUserFoodPlanDaily === idDaily);
    return day ? day.dayName : '';
  }

  adjustMacrosForSelectedDay() {
    if (!this.selectedDayId) return;
    
    this.isAdjustingMacros = true;
    this.aiMessage = ''; 
    this.aiMessageType = ''; 
    this.cdr.detectChanges(); 
      
    this.homeService.adjustDayMacros(this.selectedDayId)
      .pipe(
        timeout(60000)
      )
      .subscribe({
        next: (res) => {
          this.isAdjustingMacros = false;
          this.aiMessageType = 'success';
          this.aiMessage = "Raciones del día cuadradas correctamente";
          this.loadDashboardData(); 
          this.cdr.detectChanges();
          
          setTimeout(() => {
            this.aiMessage = '';
            this.cdr.detectChanges();
          }, 4000);
        },
        error: (err) => {
          console.error(err);
          this.isAdjustingMacros = false;
          this.aiMessageType = 'error';
          
          if (err.name === 'TimeoutError' || err.message?.includes('Timeout')) {
             this.aiMessage = "La IA está tardando demasiado en responder. Los servidores deben estar muy saturados. Por favor, inténtalo más tarde.";
          }
          else if (err.status === 503 || (typeof err.error === 'string' && err.error?.includes("high demand"))) {
            this.aiMessage = "Servidores de IA saturados por alta demanda. Por favor, espera unos segundos y vuelve a intentarlo.";
          } 
          else if (err.error && typeof err.error === 'string') {
            this.aiMessage = err.error; 
          } 
          else if (err.error && err.error.message) {
            this.aiMessage = err.error.message;
          }
          else {
            this.aiMessage = "Error de conexión con la IA. Inténtalo de nuevo.";
          }
          this.cdr.detectChanges(); 
        }
      });
  }

  getMealClass(mealName: string): string {
    if (!mealName) return '';
    const name = mealName.toLowerCase();
    if (name.includes('desayuno')) return 'meal-desayuno';
    if (name.includes('almuerzo')) return 'meal-almuerzo';
    if (name.includes('comida')) return 'meal-comida';
    if (name.includes('merienda')) return 'meal-merienda';
    if (name.includes('cena')) return 'meal-cena';
    return '';
  }

  viewRecipeDetails() {
    this.router.navigate(['/recipes']);
  }

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar() {
    this.isSidebarOpen = false;
  }

  openInfoModal(recipe: any) {
    this.selectedRecipe = recipe;
    this.isInfoModalOpen = true;
    this.cdr.detectChanges();
  }

  closeInfoModal() {
    this.isInfoModalOpen = false;
    this.selectedRecipe = null;
  }

  loadDashboardData(): void {
    this.loading = true;
    
    this.homeService.getActiveWeekCalendar(this.userId).subscribe({
      next: (cal) => {
        if (!cal?.days || cal.days.length === 0) {
          this.hasActivePlan = false;
          this.loading = false;
          this.cdr.detectChanges();
        } else {
          this.calendar = cal;
          this.hasActivePlan = true;
          
          if (this.calendar?.days) {
            this.calendar.days.forEach(day => {
              if (!this.connectedLists.includes('day-list-' + day.idUserFoodPlanDaily)) {
                this.connectedLists.push('day-list-' + day.idUserFoodPlanDaily);
              }
              this.loadDailyHeader(day.idUserFoodPlanDaily);
            });
          }
          this.loadRemainingData();
        }
      },
      error: (err) => {
        console.error(err);
        this.hasActivePlan = false;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadRemainingData(): void {
    this.homeService.getWeeklyHeader(this.userId).subscribe({
      next: (header) => {
        this.weeklyHeader = header;
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });

    this.homeService.getRecipesForUser(this.userId).subscribe({
      next: (recipes) => {
        this.availableRecipes = recipes.filter(r => !r.recipeName.startsWith('[AJUSTADO]'));
        this.loading = false; 
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadDailyHeader(idDaily: number): void {
    this.homeService.getDailyHeader(idDaily).subscribe(header => {
      this.dailyHeaders[idDaily] = header;
      this.cdr.detectChanges(); 
    });
  }

  drop(event: CdkDragDrop<any[]>, targetDayId?: number) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      if (targetDayId) {
        this.saveDayConfiguration(targetDayId, event.container.data);
      }
    } 
    else if (event.previousContainer.id === 'recipe-catalog') {
      copyArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      
      event.container.data[event.currentIndex] = { ...event.container.data[event.currentIndex] };
      if (targetDayId) {
        this.saveDayConfiguration(targetDayId, event.container.data);
      }
    } 
    else if (event.container.id === 'recipe-catalog') {
      event.previousContainer.data.splice(event.previousIndex, 1);
      const oldDayId = Number.parseInt(event.previousContainer.id.replace('day-list-', ''), 10);
      this.saveDayConfiguration(oldDayId, event.previousContainer.data); 
    } 
    else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      if (targetDayId) {
        this.saveDayConfiguration(targetDayId, event.container.data); 
      }
      const previousListId = event.previousContainer.id;
      const oldDayId = Number.parseInt(previousListId.replace('day-list-', ''), 10);
      this.saveDayConfiguration(oldDayId, event.previousContainer.data); 
    }
  }

  saveDayConfiguration(idDailyPlan: number, recipes: any[]): void {
    if (this.dailyHeaders[idDailyPlan]) {
      let totalKcal = 0; 
      let totalProt = 0; 
      let totalCarbs = 0; 
      let totalFat = 0;

      recipes.forEach(r => {
        totalKcal += r.kcal || 0;
        totalProt += r.protein || 0;
        totalCarbs += r.carbs || 0;
        totalFat += r.fat || 0;
      });

      this.dailyHeaders[idDailyPlan].realKcal = totalKcal;
      this.dailyHeaders[idDailyPlan].realProtein = Number(totalProt.toFixed(1));
      this.dailyHeaders[idDailyPlan].realCarbs = Number(totalCarbs.toFixed(1));
      this.dailyHeaders[idDailyPlan].realFat = Number(totalFat.toFixed(1));
      
      this.cdr.detectChanges(); 
    }

    const recipeIds = recipes.map(r => r.idRecipe);
    this.homeService.updateDailyRecipes({
      idUserFoodPlanDaily: idDailyPlan,
      recipeIds: recipeIds
    }).subscribe(() => {
      this.loadDailyHeader(idDailyPlan);
    });
  }

  getEmptySlots(day: any): any[] {
    if (!this.calendar || !day.assignedRecipes) return [];
    const count = this.calendar.mealsPerDay - day.assignedRecipes.length;
    return count > 0 ? new Array(count) : [];
  }

  crearNuevoPlan() {
    this.loading = true;
    this.homeService.createWeeklyPlan(this.userId).subscribe({
      next: () => {
        this.loadDashboardData();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.cdr.detectChanges();
        if (err.status === 404 || err.status === 400 || err.status === 500) {
           this.router.navigate(['/onboarding']); 
        } else {
           alert("Ha ocurrido un error inesperado al intentar crear tu plan.");
        }
      }
    });
  }
}