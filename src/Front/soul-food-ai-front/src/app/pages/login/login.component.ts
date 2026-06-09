import { Component, ChangeDetectorRef } from '@angular/core'; // 1. NUEVO: Importar ChangeDetectorRef
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service'; 

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = '';
  password = '';
  
  errorMessage = '';
  isSubmitting = false;

  constructor(
    private readonly userService: UserService, 
    private readonly router: Router,
    private readonly cdr: ChangeDetectorRef // 2. NUEVO: Inyectarlo en el constructor
  ) {}

  login() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Por favor, rellena todos los campos.';
      return;
    }

    this.isSubmitting = true; 
    this.errorMessage = '';

    this.userService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: (err) => {
        this.isSubmitting = false; 
        
        if (err.status === 401) {
          this.errorMessage = 'Email o contraseña incorrectos.';
        } else {
          this.errorMessage = 'Error al conectar con el servidor.';
        }
        
        console.error(err); 

        // 3. NUEVO: Obligamos a Angular a actualizar el HTML instantáneamente
        this.cdr.detectChanges(); 
      }
    });
  }
}