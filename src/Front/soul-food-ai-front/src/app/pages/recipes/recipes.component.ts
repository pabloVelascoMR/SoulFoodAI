import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RecipesService, CreateAiRecipeDto, AddRecipeDto } from '../../services/recipes.service';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recipes.component.html',
  styleUrls: ['./recipes.component.css']
})
export class RecipesComponent implements OnInit {
  idUser: number = 1024; // Tu usuario de pruebas

  meals: any[] = [];
  availableIngredients: any[] = [];
  userRecipesList: any[] = []; 

  isAiModalOpen: boolean = false;
  isManualModalOpen: boolean = false;

  selectedMealAi: number | null = null;
  promptTextAi: string = '';
  isLoadingAi: boolean = false;
  errorAi: string = '';
  successAi: string = '';


  manualRecipe = {
    recipeName: '',
    recipeDescription: '',
    idMeal: null as number | null,
    ingredients: [] as { idIngredient: number | null, quantity: number, unit: string }[]
  };
  isLoadingManual: boolean = false;
  errorManual: string = '';
  successManual: string = '';

  constructor(
    private recipesService: RecipesService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
  
    this.loadUserRecipes();
    this.recipesService.getMeals().subscribe(data => this.meals = data);
    this.recipesService.getAllowedIngredients(this.idUser).subscribe(data => this.availableIngredients = data);
  }

  loadUserRecipes(): void {
    this.recipesService.getUserRecipes(this.idUser).subscribe({
      next: (data) => {
        this.userRecipesList = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error cargando recetas del usuario', err)
    });
  }

  openAiModal() { this.isAiModalOpen = true; }
  closeAiModal() { 
    this.isAiModalOpen = false; 
    this.errorAi = ''; this.successAi = '';
  }

  openManualModal() { 
    this.isManualModalOpen = true; 
    if(this.manualRecipe.ingredients.length === 0) this.addIngredientRow();
  }
  closeManualModal() { 
    this.isManualModalOpen = false;
    this.errorManual = ''; this.successManual = '';
  }

  generateAiRecipe(): void {
    if (!this.selectedMealAi) {
      this.errorAi = 'Selecciona para qué comida es la receta.';
      return;
    }

    this.isLoadingAi = true;
    this.errorAi = ''; this.successAi = '';

    const dto: CreateAiRecipeDto = {
      promptDescription: this.promptTextAi,
      idMeal: Number(this.selectedMealAi)
    };

    this.recipesService.generateRecipeAI(this.idUser, dto).subscribe({
      next: (res) => {
        this.isLoadingAi = false;
        this.successAi = res.message || 'Receta creada maravillosamente.';
        this.promptTextAi = '';
        this.loadUserRecipes(); // Refrescamos el panel de fondo
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoadingAi = false;
        this.errorAi = err.error?.message || err.error || 'Error al conectar con el Chef AI.';
        this.cdr.detectChanges();
      }
    });
  }

  addIngredientRow(): void {
    this.manualRecipe.ingredients.push({ idIngredient: null, quantity: 100, unit: 'g' });
  }

  removeIngredientRow(index: number): void {
    this.manualRecipe.ingredients.splice(index, 1);
  }

  submitManualRecipe(): void {
    if (!this.manualRecipe.recipeName || !this.manualRecipe.idMeal || this.manualRecipe.ingredients.length === 0) {
      this.errorManual = 'Rellena el nombre, la comida y añade al menos un ingrediente.';
      return;
    }

    this.isLoadingManual = true;
    this.errorManual = ''; this.successManual = '';

    const dto: AddRecipeDto = {
      idMeal: Number(this.manualRecipe.idMeal),
      recipeName: this.manualRecipe.recipeName,
      recipeDescription: this.manualRecipe.recipeDescription,
      idIngredients: this.manualRecipe.ingredients.map(i => Number(i.idIngredient)),
      quantity: this.manualRecipe.ingredients.map(i => i.quantity),
      unit: this.manualRecipe.ingredients.map(i => i.unit)
    };

    this.recipesService.addRecipeManual(this.idUser, dto).subscribe({
      next: () => {
        this.isLoadingManual = false;
        this.successManual = 'Receta guardada con éxito.';
        this.loadUserRecipes(); // Refrescamos el panel de fondo 
        this.manualRecipe = { recipeName: '', recipeDescription: '', idMeal: null, ingredients: [] };
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoadingManual = false;
        this.errorManual = typeof err.error === 'string' ? err.error : 'Error al guardar la receta.';
        this.cdr.detectChanges();
      }
    });
  }
}