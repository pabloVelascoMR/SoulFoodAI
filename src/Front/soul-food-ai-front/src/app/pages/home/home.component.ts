import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { 
  HomeService, 
  WeekCalendarDto, 
  WeeklyHeaderDto, 
  DailyHeaderDto,
  RecipeCardDto
} from '../../services/home.service';

@Component({
  selector: 'app-home',
  standalone: true, 
  imports: [CommonModule, DragDropModule], 
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  
  userId: number = 12; 

  weeklyHeader: WeeklyHeaderDto | null = null;
  calendar: WeekCalendarDto | null = null;
  availableRecipes: any[] = [];
  connectedLists: string[] = ['recipe-catalog'];
  dailyHeaders: { [key: number]: DailyHeaderDto } = {};

  constructor(
    private homeService: HomeService,
    @Inject(PLATFORM_ID) private platformId: Object,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadDashboardData();
    }
  }

  loadDashboardData(): void {
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
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error("Error cargando recetas:", err)
    });

    this.homeService.getActiveWeekCalendar(this.userId).subscribe({
      next: (cal) => {
        this.calendar = cal;
        this.cdr.detectChanges(); 
       
        if (this.calendar?.days) {
          this.calendar.days.forEach(day => {
            this.connectedLists.push('day-list-' + day.idUserFoodPlanDaily);
            this.loadDailyHeader(day.idUserFoodPlanDaily);
          });
        }
      },
      error: (err) => console.error("Error cargando calendario:", err)
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
}