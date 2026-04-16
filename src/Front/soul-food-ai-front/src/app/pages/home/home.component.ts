import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { HomeService, WeekCalendarDto, WeeklyHeaderDto, DailyHeaderDto } from '../../services/home.service';

@Component({
  selector: 'app-home',
  standalone: true, 
  imports: [CommonModule, DragDropModule], 
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  userId: number = 0;

  weeklyHeader: WeeklyHeaderDto | null = null;
  calendar: WeekCalendarDto | null = null;
  availableRecipes: any[] = [];
  connectedLists: string[] = ['recipe-catalog'];
  dailyHeaders: { [key: number]: DailyHeaderDto } = {};

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

  loadDashboardData(): void {
    this.homeService.getWeeklyHeader(this.userId).subscribe({
      next: (h) => { this.weeklyHeader = h; this.cdr.detectChanges(); },
      error: (e) => console.error(e)
    });

    this.homeService.getRecipesForUser(this.userId).subscribe({
      next: (r) => { this.availableRecipes = r; this.cdr.detectChanges(); },
      error: (e) => console.error(e)
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
      error: (e) => console.error(e)
    });
  }

  loadDailyHeader(idDaily: number): void {
    this.homeService.getDailyHeader(idDaily).subscribe(h => {
      this.dailyHeaders[idDaily] = h;
      this.cdr.detectChanges();
    });
  }

  drop(event: CdkDragDrop<any[]>, targetDayId?: number) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
      
      if (targetDayId) this.saveDayConfiguration(targetDayId, event.container.data);
      
      const prevId = event.previousContainer.id;
      if (prevId.startsWith('day-list-')) {
        const oldId = parseInt(prevId.replace('day-list-', ''));
        this.saveDayConfiguration(oldId, event.previousContainer.data);
      }
    }
  }

  saveDayConfiguration(idDailyPlan: number, recipes: any[]): void {
    const recipeIds = recipes.map(r => r.idRecipe);
    this.homeService.updateDailyRecipes({ idUserFoodPlanDaily: idDailyPlan, recipeIds })
      .subscribe(() => this.loadDailyHeader(idDailyPlan));
  }

  getEmptySlots(day: any): any[] {
    if (!this.calendar || !day.assignedRecipes) return [];
    const count = this.calendar.mealsPerDay - day.assignedRecipes.length;
    return count > 0 ? new Array(count) : [];
  }
}