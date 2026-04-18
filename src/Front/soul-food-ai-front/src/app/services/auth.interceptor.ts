import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  let token = null;
  
  // 1. LA MAGIA: Solo buscamos el token si estamos en el navegador
  if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
    token = localStorage.getItem('token'); // Asegúrate de que tu token se llame así
  }

  // 2. Clonamos la petición para añadirle el token
  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(clonedReq);
  }

  // Si no hay token o estamos en el servidor, dejamos pasar la petición normal
  return next(req);
};