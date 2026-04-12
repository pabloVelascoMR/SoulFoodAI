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
  // Datos del formulario
  userData = {
    email: '',
    password: '',
    confirmPassword: ''
  };

  errorMessage: string = '';

  constructor(private userService: UserService, private router: Router) {}

  onRegister() {
   
    if (!this.userData.email.includes('@')) {
      this.errorMessage = 'Por favor, introduce un correo válido.';
      return;
    }

    if (this.userData.password !== this.userData.confirmPassword) {
      this.errorMessage = 'Las contraseñas no coinciden.';
      return;
    }

    const password = this.userData.password;
    
    if (password.length < 8) {
      this.errorMessage = 'La contraseña debe tener al menos 8 caracteres.';
      return;
    }

    if (!/[A-Z]/.test(password)) {
      this.errorMessage = 'La contraseña debe contener al menos una letra mayúscula.';
      return;
    }

    if (!/[a-z]/.test(password)) {
      this.errorMessage = 'La contraseña debe contener al menos una letra minúscula.';
      return;
    }

    if (!/[0-9]/.test(password)) {
      this.errorMessage = 'La contraseña debe contener al menos un número.';
      return;
    }

    const dto = {
      email: this.userData.email,
      password: this.userData.password
    };

    this.userService.register(dto).subscribe({
      next: (res) => {
        console.log('Usuario registrado con ID:', res.idUser);
        this.router.navigate(['/onboarding']);
      },
      error: (err) => {
        this.errorMessage = err.error || 'Usuario ya registrado';
      }
    });
  }
}