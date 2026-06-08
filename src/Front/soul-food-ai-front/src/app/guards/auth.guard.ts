import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

export const authGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const userId = userService.getUserId();

  if (userId) {
    return true;
  } else {
    router.navigate(['/']);
    return false;
  }
};