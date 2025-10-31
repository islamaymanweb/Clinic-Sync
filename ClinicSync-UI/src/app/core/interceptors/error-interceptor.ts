import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
   return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred';

      if (error.error instanceof ErrorEvent) {
        // 🧩 Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // ⚙️ Server-side error
        if (error.error?.errors && Array.isArray(error.error.errors)) {
          errorMessage = error.error.errors.join(', ');
        } else if (error.error?.message) {
          errorMessage = error.error.message;
        } else {
          errorMessage = `Error ${error.status}: ${error.message}`;
        }
      }

      console.error('HTTP Error:', errorMessage);
      return throwError(() => new Error(errorMessage));
    })
  );
};
