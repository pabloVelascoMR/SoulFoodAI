import { Component, OnInit, ChangeDetectorRef, AfterViewInit } from '@angular/core';
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

  showCustomModal: boolean = false;
  isEditing: boolean = false;
  newIngredient: any = this.getEmptyIngredient();

  constructor(
    private ingredientService: IngredientService,
    private userIngredientService: UserIngredientService,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    // Cargamos los datos normalmente
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
        this.cdr.markForCheck(); 
      }
    });
  }

  loadIngredientsForCurrentStep(): void {
    const currentStep = this.steps[this.currentStepIndex];
    this.currentIngredients = []; 

    this.ingredientService.getIngredients(currentStep.name, this.userId)
      .subscribe({
        next: (ingredients) => {
          this.currentIngredients = ingredients;
          this.cdr.markForCheck(); 
        },
        error: (error) => console.error(`Fallo al obtener ingredientes:`, error)
      });
  }

  toggleIngredientSelection(ingredient: any): void {
    const rawId = ingredient.idIngredient || ingredient.IdIngredient || ingredient.id || ingredient.Id;
    if (!rawId) return; 

    const validId: number = Number(rawId);

    if (this.selectedIngredientIds.has(validId)) {
      this.userIngredientService.deselectIngredient(this.userId, validId).subscribe({
        next: () => {
          this.selectedIngredientIds.delete(validId);
          this.cdr.detectChanges(); 
        }
      });
    } else {
      this.userIngredientService.selectIngredient(this.userId, ingredient).subscribe({
        next: () => {
          this.selectedIngredientIds.add(validId);
          this.cdr.detectChanges(); 
        }
      });
    }
  }

  isIngredientSelected(ingredient: any): boolean {
    if (!ingredient) return false;
    const rawId = ingredient.idIngredient || ingredient.IdIngredient || ingredient.id || ingredient.Id;
    return this.selectedIngredientIds.has(Number(rawId));
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

  handleImageError(event: any, ingredient: any): void {
    event.target.style.display = 'none'; 
    ingredient.hasImageError = true;     
    this.cdr.detectChanges();            
  }

  canEdit(ingredient: any): boolean {
    const isCreator = ingredient.createdByUserId === this.userId;
    const hasOFF = ingredient.openFoodFactsId || ingredient.IdOpenFoodFacts || ingredient.idOpenFoodFacts;
    return isCreator && !hasOFF;
  }

  canDelete(ingredient: any): boolean {
    const isCreator = ingredient.createdByUserId === this.userId;
    const id = Number(ingredient.idIngredient || ingredient.id);
    return isCreator && id > 191;
  }

  deleteCustomIngredient(ingredient: any, event: Event): void {
    event.stopPropagation(); 
    if (confirm(`¿Estás seguro de que quieres eliminar '${ingredient.name}'?`)) {
      const rawId = ingredient.idIngredient || ingredient.id;
      this.ingredientService.deleteCustomIngredient(Number(rawId), this.userId).subscribe({
        next: () => this.loadIngredientsForCurrentStep()
      });
    }
  }

  getEmptyIngredient() {
    return { name: '', brand: '', category: '', icon: '🍽️', protein: 0, carbs: 0, fat: 0, kcal: 0 };
  }

  openCustomModal(): void {
    this.isEditing = false;
    this.newIngredient = this.getEmptyIngredient();
    this.newIngredient.category = this.steps[this.currentStepIndex].name;
    this.showCustomModal = true;
  }

  editCustomIngredient(ingredient: any, event: Event): void {
    event.stopPropagation();
    this.isEditing = true;
    this.newIngredient = {
      id: ingredient.idIngredient || ingredient.id,
      name: ingredient.name,
      brand: ingredient.brand,
      category: ingredient.category,
      icon: ingredient.icon,
      protein: ingredient.protein,
      carbs: ingredient.carbs,
      fat: ingredient.fat,
      kcal: ingredient.kcal
    };
    this.showCustomModal = true;
  }

  closeCustomModal(): void {
    this.showCustomModal = false;
  }

  saveCustomIngredient(): void {
    if (!this.newIngredient.name || this.newIngredient.name.trim() === '') return;

    const dto = {
      id: this.newIngredient.id,
      name: this.newIngredient.name,
      brand: this.newIngredient.brand,
      category: this.newIngredient.category,
      icon: this.newIngredient.icon,
      protein: this.newIngredient.protein,
      carbs: this.newIngredient.carbs,
      fat: this.newIngredient.fat,
      kcal: this.newIngredient.kcal,
      userId: this.userId
    };

    if (this.isEditing) {
      this.ingredientService.updateCustomIngredient(this.newIngredient.id, dto).subscribe({
        next: () => {
          this.closeCustomModal();
          this.loadIngredientsForCurrentStep();
        }
      });
    } else {
      this.ingredientService.addCustomIngredient(dto).subscribe({
        next: () => {
          this.closeCustomModal();
          this.loadIngredientsForCurrentStep(); 
          this.loadSelectedIngredients(); 
        }
      });
    }
  }
}