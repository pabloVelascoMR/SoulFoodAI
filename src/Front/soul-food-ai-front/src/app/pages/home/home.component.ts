import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem, copyArrayItem } from '@angular/cdk/drag-drop';
import { Router, RouterModule } from '@angular/router'; 
import { UserService } from '../../services/user.service';
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

  constructor(
    private homeService: HomeService,
    private userService: UserService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object,
    private cdr: ChangeDetectorRef 
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

  // --- FUNCIONES DE SELECCIÓN DE DÍA  IA ---
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
    
    this.homeService.adjustDayMacros(this.selectedDayId).subscribe({
      next: (res) => {
        this.isAdjustingMacros = false;
        alert("¡Magia! Macros cuadrados perfectamente.");
        this.loadDashboardData(); 
      },
      error: (err) => {
        this.isAdjustingMacros = false;
        alert("La IA dice: " + (err.error || "Error al cuadrar macros"));
      }
    });
  }

  // --- FUNCIONES DE ESTÉTICA Y NAVEGACIÓN ---
  getMealClass(mealName: string): string {
    if (!mealName) return '';
    const name = mealName.toLowerCase();
    if (name.includes('desayuno')) return 'meal-yellow';
    if (name.includes('almuerzo')) return 'meal-orange';
    if (name.includes('comida')) return 'meal-red';
    if (name.includes('merienda')) return 'meal-purple';
    if (name.includes('cena')) return 'meal-blue';
    return '';
  }

  viewRecipeDetails() {
    this.router.navigate(['/recipes']);
  }

  // --- FUNCIONES DEL MENÚ LATERAL ---
  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar() {
    this.isSidebarOpen = false;
  }

  // --- CARGA DE DATOS ---
  loadDashboardData(): void {
    this.loading = true;
    
    this.homeService.getActiveWeekCalendar(this.userId).subscribe({
      next: (cal) => {
        if (!cal || !cal.days || cal.days.length === 0) {
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
        console.error("Error cargando calendario (posiblemente no hay plan):", err);
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
      error: (err) => console.error("Error cargando cabecera:", err)
    });

    this.homeService.getRecipesForUser(this.userId).subscribe({
      next: (recipes) => {
        // Filtramos para que no salgan las recetas clonadas de ajuste en el catálogo
        this.availableRecipes = recipes.filter(r => !r.recipeName.startsWith('[AJUSTADO]'));
        this.loading = false; 
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        console.error("Error cargando recetas:", err);
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

  // --- DRAG AND DROP ---
  drop(event: CdkDragDrop<any[]>, targetDayId?: number) {
    
    // 1. Si mueves una tarjeta dentro del mismo día o dentro del catálogo
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      if (targetDayId) {
        this.saveDayConfiguration(targetDayId, event.container.data);
      }
    } 
    else {
      if (event.previousContainer.id === 'recipe-catalog') {
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
        event.previousContainer.data.splice(event.previousIndex, 1); // Borra la tarjeta visualmente
        
        const oldDayId = parseInt(event.previousContainer.id.replace('day-list-', ''), 10);
        this.saveDayConfiguration(oldDayId, event.previousContainer.data); // Guarda el día sin esa receta
      } 

      else {
        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex,
        );

        if (targetDayId) {
          this.saveDayConfiguration(targetDayId, event.container.data); // Guarda el día destino
        }

        const previousListId = event.previousContainer.id;
        const oldDayId = parseInt(previousListId.replace('day-list-', ''), 10);
        this.saveDayConfiguration(oldDayId, event.previousContainer.data); // Guarda el día origen
      }
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
        console.error("Error al crear plan:", err);
        this.loading = false;
        this.cdr.detectChanges();

        if (err.status === 404 || err.status === 400 || err.status === 500) {
           console.warn("Redirigiendo al Onboarding...");
           this.router.navigate(['/onboarding']); 
        } else {
           alert("Ha ocurrido un error inesperado al intentar crear tu plan.");
        }
      }
    });
  }
}