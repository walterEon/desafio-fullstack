import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  const request = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(request).pipe(
    catchError((error: unknown) => {
      // Un 401 en el login son credenciales inválidas; lo maneja la pantalla de login.
      const isLoginCall = req.url.endsWith('/api/auth/login');
      if (error instanceof HttpErrorResponse && error.status === 401 && !isLoginCall) {
        auth.logout();
      }
      return throwError(() => error);
    }),
  );
};
