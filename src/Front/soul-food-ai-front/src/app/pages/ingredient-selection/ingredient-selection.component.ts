import { Component, OnInit } from '@angular/core';
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

  constructor(
    private ingredientService: IngredientService,
    private userIngredientService: UserIngredientService
  ) {}

  ngOnInit(): void {
    console.log("Componente inicializado");
  }
}