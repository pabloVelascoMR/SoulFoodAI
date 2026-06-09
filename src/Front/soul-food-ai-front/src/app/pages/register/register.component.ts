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
  
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = ''; 
  errorMessage: string = '';
  termsAccepted: boolean = false;
  isSubmitting: boolean = false; 

  constructor(private readonly userService: UserService, private readonly router: Router) {}

  register() {
    if (this.isSubmitting) return; 

    this.errorMessage = '';

    if (!this.username || !this.email || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Todos los campos son obligatorios.';
      return;
    }

    if (!this.termsAccepted) {
      this.errorMessage = 'Debes aceptar la Política de Privacidad y el tratamiento de datos.';
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

    const pass = this.password;
    if (pass.length < 8 || !/[A-Z]/.test(pass) || !/\d/.test(pass)) {
      this.errorMessage = 'Contraseña: mín. 8 caracteres, una mayúscula y un número.';
      return;
    }

    const dto = {
      userName: this.username,
      email: this.email,
      passwordHash: this.password
    };

    this.isSubmitting = true; 

    this.userService.register(dto).subscribe({
      next: (res) => {
        this.router.navigate(['/onboarding']); 
      },
      error: (err) => {
        this.isSubmitting = false; 
        console.error("Error completo del servidor:", err);
        this.errorMessage = err.error?.message || err.error || 'Error del servidor (500). Revisa la consola de C#.';
      }
    });
  }
}