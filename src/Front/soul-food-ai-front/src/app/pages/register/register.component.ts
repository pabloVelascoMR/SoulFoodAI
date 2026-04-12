import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  
  // Variables para el formulario
  username: string = ''; // 🔴 Nuevo campo
  email: string = '';
  password: string = '';
  confirmPassword: string = ''; 
  
  errorMessage: string = '';

  constructor(private userService: UserService, private router: Router) {}

  register() {
    this.errorMessage = '';

    // Validaciones básicas
    if (!this.username || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Todos los campos son obligatorios.';
      return;
    }

    if (!this.email.includes('@')) {
      this.errorMessage = 'Introduce un email válido.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Las contraseñas no coinciden.';
      return;
    }

    // Validación de seguridad (8 caracteres, Mayus, Num)
    const pass = this.password;
    if (pass.length < 8 || !/[A-Z]/.test(pass) || !/[0-9]/.test(pass)) {
      this.errorMessage = 'Contraseña: mín. 8 caracteres, una mayúscula y un número.';
      return;
    }

    const dto = {
      userName: this.username,         
      email: this.email,
      passwordHash: this.password      
    };

    this.userService.register(dto).subscribe({
      next: (res) => {
        this.router.navigate(['/onboarding']);
      },
      error: (err) => {
        this.errorMessage = err.error || 'Error al registrar usuario.';
      }
    });
  }
}