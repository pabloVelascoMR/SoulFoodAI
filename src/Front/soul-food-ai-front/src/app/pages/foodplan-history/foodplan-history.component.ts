import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FoodplanHistoryService } from '../../services/foodplan-history.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-foodplan-history',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './foodplan-history.component.html',
  styleUrls: ['./foodplan-history.component.css']
})
export class FoodplanHistoryComponent implements OnInit {
  historyPlans: any[] = [];
  isLoading: boolean = true;
  errorMessage: string = '';
  planToHide: number | null = null; 

  constructor(
    private historyService: FoodplanHistoryService, 
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory() {
    const userId = Number(localStorage.getItem('userId')) || 1; 

    this.isLoading = true;
    this.errorMessage = '';

    this.historyService.getPlanHistory(userId).subscribe({
      next: (data) => {
        this.ngZone.run(() => {
          try {
            if (!data || data.length === 0) {
              this.historyPlans = [];
              this.errorMessage = 'No tienes historial de planes con recetas registradas.';
            } else {
              this.historyPlans = data.map(plan => {
                const rawRecipes = plan.recipesEaten || [];
                const startDate = plan.startDate ? new Date(plan.startDate) : new Date();
                
                const calendarDays = [];
                for(let i = 0; i < 7; i++) {
                  const currentDate = new Date(startDate);
                  currentDate.setDate(startDate.getDate() + i); 
                  
                  const dayRecipes = rawRecipes.filter((r: any) => {
                    if (!r || !r.dateEaten) return false;
                    const rDate = new Date(r.dateEaten);
                    return rDate.getDate() === currentDate.getDate() && 
                           rDate.getMonth() === currentDate.getMonth() &&
                           rDate.getFullYear() === currentDate.getFullYear();
                  });

                  calendarDays.push({
                    date: currentDate,
                    dayName: `Día ${i + 1}`,
                    recipes: dayRecipes
                  });
                }
                return { ...plan, calendarDays };
              });
            }
            this.isLoading = false;
          } catch (error: any) {
            this.errorMessage = 'Error organizando el calendario: ' + error.message;
            this.isLoading = false;
          }
          this.cdr.detectChanges(); 
        });
      },
      error: (err) => {
        this.ngZone.run(() => {
          if (err.status === 404) {
            this.errorMessage = 'No tienes historial de planes con recetas registradas.';
          } else {
            this.errorMessage = 'Hubo un error al conectar con el servidor.';
          }
          this.isLoading = false;
          this.cdr.detectChanges(); 
        });
      }
    });
  }

  confirmHide(planId: number, event: Event) {
    event.preventDefault(); 
    this.planToHide = planId;
  }

  cancelHide() {
    this.planToHide = null;
  }

  executeHide() {
    if (this.planToHide === null) return;
    
    const planId = this.planToHide;

    this.historyService.hidePlanFromHistory(planId).subscribe({
      next: () => {
        this.ngZone.run(() => {
          this.historyPlans = this.historyPlans.filter(p => p.idUserFoodPlanWeek !== planId);
          if (this.historyPlans.length === 0) {
            this.errorMessage = 'No tienes historial de planes con recetas registradas.';
          }
          this.planToHide = null;  
          this.cdr.detectChanges();
        });
      },
      error: () => {
        this.ngZone.run(() => {
          alert('No se pudo ocultar el plan. Inténtalo de nuevo.');
          this.planToHide = null; 
          this.cdr.detectChanges();
        });
      }
    });
  }

  getMealClass(mealName: string): string {
    if (!mealName) return '';
    const name = mealName.toLowerCase();
    if (name.includes('desayuno')) return 'meal-yellow';
    if (name.includes('comida') || name.includes('almuerzo')) return 'meal-orange';
    if (name.includes('cena')) return 'meal-blue';
    if (name.includes('snack') || name.includes('merienda')) return 'meal-purple';
    return 'meal-red';
  }
}