import { Component, OnInit, ChangeDetectorRef, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common'; // <-- Añadido isPlatformBrowser
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router'; // <-- Añadido Router y RouterModule
import { RecipesService, CreateAiRecipeDto, AddRecipeDto } from '../../services/recipes.service';
import { UserService } from '../../services/user.service'; // <-- Añadido tu UserService

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // <-- Asegúrate de tener RouterModule aquí
  templateUrl: './recipes.component.html',
  styleUrls: ['./recipes.component.css']
})
export class RecipesComponent implements OnInit {
  idUser: number = 0; // Ahora empieza en 0 y se llena dinámicamente

  meals: any[] = [];
  availableIngredients: any[] = [];
  userRecipesList: any[] = [];

  isAiModalOpen: boolean = false;
  isManualModalOpen: boolean = false;
  isInfoModalOpen: boolean = false;
  selectedRecipe: any = null;
  isDeleteModalOpen: boolean = false;
  recipeToDeleteId: number | null = null;
  isDeleting: boolean = false;

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
    private cdr: ChangeDetectorRef,
    private userService: UserService, 
    private router: Router, 
    @Inject(PLATFORM_ID) private platformId: Object 
  ) {}

  ngOnInit(): void {
    
    if (isPlatformBrowser(this.platformId)) {
      const id = this.userService.getUserId();
      if (!id) {
        this.router.navigate(['/login']);
        return;
      }
      this.idUser = id; // Asignamos el ID real del usuario
      this.loadInitialData();
    }
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
      error: (err) => console.error(err)
    });
  }

  getMealClass(mealName: string): string {
  if (!mealName) return '';
  const name = mealName.toLowerCase();
  if (name.includes('desayuno')) return 'meal-desayuno';
  if (name.includes('almuerzo')) return 'meal-almuerzo';
  if (name.includes('comida')) return 'meal-comida';
  if (name.includes('merienda')) return 'meal-merienda';
  if (name.includes('cena')) return 'meal-cena';
  return '';
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

  openInfoModal(recipe: any) {
    this.selectedRecipe = recipe;
    this.isInfoModalOpen = true;
    this.cdr.detectChanges();
  }
  closeInfoModal() {
    this.isInfoModalOpen = false;
    this.selectedRecipe = null;
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
        this.successAi = res.message;
        this.promptTextAi = '';
        this.loadUserRecipes();
        this.cdr.detectChanges();
        setTimeout(() => {
          this.closeAiModal();
          this.cdr.detectChanges(); 
        }, 0); 
      },
      error: (err) => {
        this.isLoadingAi = false;
        this.errorAi = err.error?.message || err.error;
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
      this.errorManual = 'Completa los campos obligatorios.';
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
        this.successManual = 'Receta guardada.';
        this.loadUserRecipes();
        this.manualRecipe = { recipeName: '', recipeDescription: '', idMeal: null, ingredients: [] };
        this.cdr.detectChanges();

        setTimeout(() => {
          this.closeManualModal();
          this.cdr.detectChanges(); 
        }, 0);
      },
      error: (err) => {
        this.isLoadingManual = false;
        this.errorManual = err.error;
        this.cdr.detectChanges();
      }
    });
  }

  archiveRecipe(idRecipe: number, event: Event): void {
    event.stopPropagation(); 

    if (confirm('¿Estás seguro de que quieres borrar esta receta? No aparecerá más, pero se mantendrá en tus planes históricos.')) {
      this.recipesService.archiveRecipe(idRecipe).subscribe({
        next: (res) => {
          
          this.loadUserRecipes(); 
          console.log("Receta archivada con éxito.");
        },
        error: (err) => {
          console.error("Error al archivar la receta:", err);
          alert("Hubo un error al intentar borrar la receta.");
        }
      });
    }
  }

  openDeleteModal(idRecipe: number, event: Event): void {
    event.stopPropagation(); // Evita que se abra la otra modal de info
    this.recipeToDeleteId = idRecipe;
    this.isDeleteModalOpen = true;
  }

  closeDeleteModal(): void {
    this.isDeleteModalOpen = false;
    this.recipeToDeleteId = null;
  }
  
  confirmDeleteRecipe(): void {
    if (!this.recipeToDeleteId) return;

    this.isDeleting = true; // Activamos el spinner

    this.recipesService.archiveRecipe(this.recipeToDeleteId).subscribe({
      next: (res) => {
        this.isDeleting = false;
        this.loadUserRecipes(); // Recargamos para que desaparezca
        this.closeDeleteModal(); // Cerramos la modal
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("Error al archivar la receta:", err);
        this.isDeleting = false;
        alert("Hubo un error al intentar borrar la receta."); // Por si falla el servidor
        this.closeDeleteModal();
        this.cdr.detectChanges();
      }
    });
  }
}