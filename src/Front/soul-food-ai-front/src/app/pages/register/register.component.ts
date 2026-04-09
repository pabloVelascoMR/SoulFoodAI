import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router'; 
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  name = '';
  email = '';
  password = '';

  
  constructor(private userService: UserService, private router: Router) {}

  register() {

    const user = {
      userName: this.name,
      email: this.email,
      passwordHash: this.password
    };

    this.userService.register(user).subscribe({
      next: (res) => {
        console.log("Usuario creado:", res);
        
        this.router.navigate(['/onboarding']);
      },
      error: (err) => {
        console.error(err);
        alert("Error al registrar. Revisa los datos o prueba con otro email.");
      }
    });
  }
}