import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // <-- 1. AÑADIMOS ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RecipesService, CreateAiRecipeDto } from '../../services/recipes.service';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './recipes.component.html',
  styleUrls: ['./recipes.component.css']
})
export class RecipesComponent implements OnInit {
  idUser: number = 12; 

  meals: any[] = [];
  selectedMeal: number | null = null;
  promptText: string = '';

  isLoading: boolean = false;
  successMessage: string = '';
  generatedRecipeName: string = '';
  errorMessage: string = '';

  constructor(
    private recipesService: RecipesService,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    this.recipesService.getMeals().subscribe({
      next: (data) => {
        this.meals = data;
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error('Error cargando meals', err)
    });
  }

  generateRecipe(): void {
    if (!this.selectedMeal) {
      this.errorMessage = 'Por favor, selecciona para qué comida es la receta.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const dto: CreateAiRecipeDto = {
      promptDescription: this.promptText,
      idMeal: Number(this.selectedMeal)
    };

    this.recipesService.generateRecipeAI(this.idUser, dto).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.successMessage = response.message || 'Receta creada maravillosamente.';
        this.generatedRecipeName = response.recipeName;
        this.promptText = '';
        this.cdr.detectChanges(); 
      },
      error: (err) => {
        this.isLoading = false; 
        
        if (err.error && err.error.message) {
          this.errorMessage = err.error.message; 
        } else if (typeof err.error === 'string') {
          this.errorMessage = err.error;
        } else {
          this.errorMessage = 'Hubo un problema de conexión al generar la receta. Inténtalo de nuevo.';
        }
        console.error('Error del Chef AI:', err);
        this.cdr.detectChanges();
      }
    });
  }
}