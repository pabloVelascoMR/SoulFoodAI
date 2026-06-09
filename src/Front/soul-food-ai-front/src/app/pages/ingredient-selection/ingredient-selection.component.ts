import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { IngredientService } from '../../services/ingredient.service';
import { UserIngredientService } from '../../services/user_ingredient.service';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ingredient-selection',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ingredient-selection.component.html',
  styleUrl: './ingredient-selection.component.css'
})
export class IngredientSelectionComponent implements OnInit {

  allSteps = [
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

  steps: any[] = [];
  currentStepIndex = 0;
  userId: number = 0;
  userDietType: any = '1';

  MIN_GENERAL = 35;
  MIN_PESCATARIAN = 30;
  MIN_VEGETARIAN = 25;
  MIN_VEGAN = 20;

  currentIngredients: any[] = [];
  selectedIngredientIds: Set<number> = new Set();

  showCustomModal: boolean = false;
  isEditing: boolean = false;
  newIngredient: any = this.getEmptyIngredient();

  showOFFModal: boolean = false;
  offSearchQuery: string = '';
  offSearchResults: any[] = [];
  isSearchingOFF: boolean = false;

  constructor(
    private readonly ingredientService: IngredientService,
    private readonly userIngredientService: UserIngredientService,
    private readonly router: Router,
    private readonly userService: UserService,
    private readonly cdr: ChangeDetectorRef,
    private readonly http: HttpClient
  ) {}

  ngOnInit(): void {
    const id = this.userService.getUserId();
    if (!id) {
      alert("Debes iniciar sesión para acceder a esta pantalla.");
      this.router.navigate(['/login']);
      return;
    }
    this.userId = id;
    this.loadUserDataAndSetup();
  }

  loadUserDataAndSetup(): void {
    this.http.get<any>(`https://localhost:7007/api/UserData/GetUserDataById/${this.userId}`).subscribe({
      next: (userData) => {
        this.userDietType = userData?.idFoodPlan || userData?.IdFoodPlan || '1';
        this.setupSteps();
        this.loadSelectedIngredients();
        this.loadIngredientsForCurrentStep();
      },
      error: (err) => {
        this.userDietType = '1';
        this.setupSteps();
        this.loadSelectedIngredients();
        this.loadIngredientsForCurrentStep();
      }
    });
  }

  setupSteps(): void {
    const diet = String(this.userDietType).toLowerCase();

    if (diet === '7' || diet.includes('vegana')) {
      this.steps = this.allSteps.filter(s =>
        s.name !== 'Carne' && s.name !== 'Pescado/Marisco' && s.name !== 'Lácteos' && s.name !== 'Quesos'
      );
    } else if (diet === '8' || diet.includes('vegetariana')) {
      this.steps = this.allSteps.filter(s =>
        s.name !== 'Carne' && s.name !== 'Pescado/Marisco'
      );
    } else if (diet === '9' || diet.includes('pesce') || diet.includes('pescata')) {
      this.steps = this.allSteps.filter(s => s.name !== 'Carne');
    } else {
      this.steps = [...this.allSteps];
    }
  }

  get requiredMinimum(): number {
    const diet = String(this.userDietType).toLowerCase();
    if (diet === '7' || diet.includes('vegana')) return this.MIN_VEGAN;
    if (diet === '8' || diet.includes('vegetariana')) return this.MIN_VEGETARIAN;
    if (diet === '9' || diet.includes('pesce') || diet.includes('pescata')) return this.MIN_PESCATARIAN;
    return this.MIN_GENERAL;
  }

  get canFinish(): boolean {
    return this.selectedIngredientIds.size >= this.requiredMinimum;
  }

  finalizar(): void {
    if (this.canFinish) {
      this.router.navigate(['/home']);
    } else {
      alert(`Necesitas seleccionar al menos ${this.requiredMinimum} ingredientes.`);
    }
  }

  loadSelectedIngredients(): void {
    this.userIngredientService.getSelectedIngredients(this.userId).subscribe({
      next: (favorites) => {
        const validIds = favorites.map((f: any) => Number(f.idIngredient || f.IdIngredient || f.id || f.Id)).filter((id: number) => !Number.isNaN(id) && id > 0);
        this.selectedIngredientIds = new Set(validIds);
        this.cdr.markForCheck();
      }
    });
  }

  loadIngredientsForCurrentStep(): void {
    if (this.steps.length === 0) return;
    const currentStep = this.steps[this.currentStepIndex];
    this.currentIngredients = [];

    this.ingredientService.getIngredients(currentStep.name, this.userId).subscribe({
      next: (ingredients) => {
        this.currentIngredients = ingredients;
        this.cdr.markForCheck();
      }
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
    const numericId = Number(ingredient.idIngredient || ingredient.id);
    if (numericId > 0 && numericId <= 191) return `/assets/ingredientes_imagenes/${numericId}.jpg`;
    if (ingredient.imageUrl?.startsWith('http')) return ingredient.imageUrl;
    return null;
  }

  handleImageError(event: any, ingredient: any): void {
    event.target.style.display = 'none';
    ingredient.hasImageError = true;
    this.cdr.detectChanges();
  }

  canEdit(ingredient: any): boolean {
    const isCreator = ingredient.createdByUserId === this.userId;
    const hasOFF = ingredient.idOpenFoodFacts || ingredient.IdOpenFoodFacts || ingredient.openFoodFactsId;
    return isCreator && !hasOFF;
  }

  canDelete(ingredient: any): boolean {
    const isCreator = ingredient.createdByUserId === this.userId;
    const id = Number(ingredient.idIngredient || ingredient.id);
    return isCreator && id > 191;
  }

  deleteCustomIngredient(ingredient: any, event: Event): void {
    event.stopPropagation();
    if (confirm(`¿Estás seguro de que quieres eliminar '${ingredient.name}' definitivamente?`)) {
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

  closeCustomModal(): void { this.showCustomModal = false; }

  saveCustomIngredient(): void {
    if (!this.newIngredient.name || this.newIngredient.name.trim() === '') return;
    const dto = {
      id: this.newIngredient.id, name: this.newIngredient.name, brand: this.newIngredient.brand,
      category: this.newIngredient.category, icon: this.newIngredient.icon, protein: this.newIngredient.protein,
      carbs: this.newIngredient.carbs, fat: this.newIngredient.fat, kcal: this.newIngredient.kcal, userId: this.userId
    };

    if (this.isEditing) {
      this.ingredientService.updateCustomIngredient(this.newIngredient.id, dto).subscribe({
        next: () => { this.closeCustomModal(); this.loadIngredientsForCurrentStep(); }
      });
    } else {
      this.ingredientService.addCustomIngredient(dto).subscribe({
        next: () => { this.closeCustomModal(); this.loadIngredientsForCurrentStep(); this.loadSelectedIngredients(); }
      });
    }
  }

  openOFFModal(): void {
    this.offSearchQuery = ''; this.offSearchResults = []; this.showOFFModal = true;
  }
  closeOFFModal(): void { this.showOFFModal = false; }

  searchOFF(): void {
    if (!this.offSearchQuery || this.offSearchQuery.trim() === '') return;
    this.isSearchingOFF = true;
    this.cdr.markForCheck();

    this.ingredientService.searchOpenFoodFacts(this.offSearchQuery).subscribe({
      next: (response: any) => {
        let products = Array.isArray(response) ? response : (response.products || []);
        this.offSearchResults = products;
        this.isSearchingOFF = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.isSearchingOFF = false;
        this.cdr.markForCheck();
      }
    });
  }

  selectOFFIngredient(product: any): void {
    const dto = {
      idUser: this.userId,
      idOpenFoodFacts: product.openFoodFactsId,
      name: product.name,
      brand: product.brand,
      imageUrl: product.imageUrl,
      category: this.steps[this.currentStepIndex].name,
      protein: product.protein,
      carbs: product.carbs,
      fat: product.fat,
      kcal: product.kcal
    };

    this.ingredientService.addSearchedIngredient(dto).subscribe({
      next: () => { this.closeOFFModal(); this.loadIngredientsForCurrentStep(); this.loadSelectedIngredients(); }
    });
  }

  trackByIngredient(index: number, ingredient: any): number {
    return ingredient.idIngredient || ingredient.id;
  }
}