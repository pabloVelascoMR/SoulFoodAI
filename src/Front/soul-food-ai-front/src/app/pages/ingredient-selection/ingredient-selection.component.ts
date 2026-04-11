import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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
    { name: 'Cereales', subcategories: [] }
  ];

  currentStepIndex = 0;
  userId: number = 5; 
  currentIngredients: any[] = [];
  
  selectedIngredientIds: Set<number> = new Set();

  constructor(
    private ingredientService: IngredientService,
    private userIngredientService: UserIngredientService,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    this.loadSelectedIngredients(); 
    this.loadIngredientsForCurrentStep();
  }

  loadSelectedIngredients(): void {
    this.userIngredientService.getSelectedIngredients(this.userId).subscribe({
      next: (favorites) => {
        const validIds = favorites.map((f: any) => {
          const rawId = f.idIngredient || f.IdIngredient || f.id || f.Id;
          return Number(rawId);
        }).filter(id => !isNaN(id) && id > 0); 

        this.selectedIngredientIds = new Set(validIds);
        this.cdr.detectChanges(); 
      }
    });
  }

  loadIngredientsForCurrentStep(): void {
    const currentStep = this.steps[this.currentStepIndex];
    this.currentIngredients = []; 
    this.cdr.detectChanges();

    this.ingredientService.getIngredients(currentStep.name, this.userId)
      .subscribe({
        next: (ingredients) => {
          this.currentIngredients = ingredients;
          this.cdr.detectChanges(); 
        },
        error: (error) => console.error(`Fallo al obtener ingredientes:`, error)
      });
  }

  // --- Lógica de Selección ---
  toggleIngredientSelection(ingredient: any): void {
    const rawId = ingredient.idIngredient || ingredient.IdIngredient || ingredient.id || ingredient.Id;
    if (!rawId) return; 

    const validId: number = Number(rawId);

    if (this.selectedIngredientIds.has(validId)) {
      this.userIngredientService.deselectIngredient(this.userId, validId).subscribe({
        next: () => {
          this.selectedIngredientIds.delete(validId);
          this.cdr.detectChanges(); 
        },
        error: (err) => console.error("Error al eliminar relación:", err)
      });
    } else {
      this.userIngredientService.selectIngredient(this.userId, ingredient).subscribe({
        next: () => {
          this.selectedIngredientIds.add(validId);
          this.cdr.detectChanges(); 
        },
        error: (err) => console.error("Error al añadir relación:", err)
      });
    }
  }

  isIngredientSelected(ingredient: any): boolean {
    if (!ingredient) return false;
    const rawId = ingredient.idIngredient || ingredient.IdIngredient || ingredient.id || ingredient.Id;
    if (!rawId) return false;

    return this.selectedIngredientIds.has(Number(rawId));
  }

  // --- Navegación ---
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

  // --- Lógica de Imágenes Locales ---
  getIngredientImage(ingredient: any): string | null {
    const id = ingredient.idIngredient || ingredient.id;
    const numericId = Number(id);

    if (numericId > 0 && numericId <= 191) {
      return `/assets/ingredientes_imagenes/${numericId}.jpg`; 
    }

    if (ingredient.imageUrl && ingredient.imageUrl.startsWith('http')) {
      return ingredient.imageUrl;
    }

    return null;
  }

  onImageError(event: any, ingredient: any): void {
    event.target.style.display = 'none'; 
    ingredient.hasImageError = true;     
  }
  
  handleImageError(event: any, ingredient: any): void {
    event.target.style.display = 'none'; 
    ingredient.hasImageError = true;     
    this.cdr.detectChanges();  
  }
}