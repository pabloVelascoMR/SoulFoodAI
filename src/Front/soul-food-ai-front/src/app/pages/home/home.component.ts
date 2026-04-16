import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Router, RouterModule } from '@angular/router'; // Asegúrate de importar RouterModule
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
  imports: [CommonModule, DragDropModule, RouterModule], // Añadido RouterModule para el menú
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  
  // --- VARIABLES DE DATOS ---
  userId: number = 0; 
  hasActivePlan: boolean = false;
  loading: boolean = true;

  weeklyHeader: WeeklyHeaderDto | null = null;
  calendar: WeekCalendarDto | null = null;
  availableRecipes: any[] = [];
  connectedLists: string[] = ['recipe-catalog'];
  dailyHeaders: { [key: number]: DailyHeaderDto } = {};

  // --- VARIABLES DEL MENÚ LATERAL ---
  isSidebarOpen: boolean = false;

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
              this.connectedLists.push('day-list-' + day.idUserFoodPlanDaily);
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
        this.availableRecipes = recipes;
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
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      if (targetDayId) {
        this.saveDayConfiguration(targetDayId, event.container.data);
      }
    } else {
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
      if (previousListId.startsWith('day-list-')) {
        const oldDayId = parseInt(previousListId.replace('day-list-', ''), 10);
        this.saveDayConfiguration(oldDayId, event.previousContainer.data);
      }
    }
  }

  saveDayConfiguration(idDailyPlan: number, recipes: any[]): void {
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