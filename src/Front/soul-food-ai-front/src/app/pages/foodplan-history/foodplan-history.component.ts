import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FoodplanHistoryService, PlanHistory } from '../../services/foodplan-history.service';

@Component({
  selector: 'app-foodplan-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './foodplan-history.component.html',
  styleUrls: ['./foodplan-history.component.css']
})
export class FoodplanHistoryComponent implements OnInit {
  historyPlans: PlanHistory[] = [];
  isLoading: boolean = true;
  errorMessage: string = '';

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
    this.historyService.getPlanHistory(userId).subscribe({
      next: (data) => {
        this.ngZone.run(() => {
          console.log(" DATOS RECIBIDOS DEL BACKEND:", data);
          
          this.historyPlans = data;
          this.isLoading = false;
          
          if (data.length === 0) {
            this.errorMessage = 'No tienes historial de planes con recetas registradas.';
          }
          this.cdr.detectChanges(); 
        });
      },
      error: (err) => {
        this.ngZone.run(() => {
          console.error("ERROR DEL BACKEND:", err);
          
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

  hidePlan(planId: number) {
    if (confirm('¿Estás seguro de que quieres ocultar este plan de tu historial?')) {
      this.historyService.hidePlanFromHistory(planId).subscribe({
        next: () => {
          this.ngZone.run(() => {
            this.historyPlans = this.historyPlans.filter(p => p.idUserFoodPlanWeek !== planId);
            
            if (this.historyPlans.length === 0) {
              this.errorMessage = 'No tienes historial de planes con recetas registradas.';
            }
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.ngZone.run(() => {
            alert('No se pudo ocultar el plan. Inténtalo de nuevo.');
            this.cdr.detectChanges();
          });
        }
      });
    }
  }
}