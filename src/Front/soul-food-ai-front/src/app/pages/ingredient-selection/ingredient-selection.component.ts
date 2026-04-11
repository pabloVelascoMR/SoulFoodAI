import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';   
import { IngredientService } from '../../services/ingredient.service';
import { UserIngredientService } from '../../services/user_ingredient.service';

@Component({
  selector: 'app-ingredient-selection',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './ingredient-selection.component.html',
  styleUrl: './ingredient-selection.component.css'
})
export class IngredientSelectionComponent implements OnInit {

  steps = [
    { name: 'Carne', subcategories: [] },
    { name: 'Pescado/Marisco', subcategories: [] }, 
    { name: 'Verdura', subcategories: [] },
    { name: 'Legumbre', subcategories: [] },
    { name: 'Hortaliza', subcategories: [] },
    { name: 'Fruta', subcategories: [] },
    { name: 'Lácteos', subcategories: [] },
    { name: 'Quesos', subcategories: [] },
    { name: 'Cereales', subcategories: [] }
  ];

  currentStepIndex = 0;
  userId: number = 5; 
  currentIngredients: any[] = [];

  constructor(
    private ingredientService: IngredientService,
    private userIngredientService: UserIngredientService,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    this.loadIngredientsForCurrentStep();
  }

  loadIngredientsForCurrentStep(): void {
    const currentStep = this.steps[this.currentStepIndex];
  
    this.currentIngredients = []; 
    this.cdr.detectChanges();

    this.ingredientService.getIngredients(currentStep.name, this.userId)
      .subscribe({
        next: (ingredients) => {
          this.currentIngredients = ingredients;
          this.cdr.detectChanges(); // Forzamos el pintado
        },
        error: (error) => {
          console.error(`Fallo al obtener ingredientes de ${currentStep.name}:`, error);
        }
      });
  }

  nextStep(): void {
    if (this.currentStepIndex < this.steps.length - 1) {
      this.currentStepIndex++;
      this.loadIngredientsForCurrentStep();
    }
  }

  previousStep(): void {
    if (this.currentStepIndex > 0) {
      this.currentStepIndex--;
      this.loadIngredientsForCurrentStep();
    }
  }
}