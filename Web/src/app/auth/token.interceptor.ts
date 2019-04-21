import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { Observable, throwError } from 'rxjs';
import { mergeMap, catchError } from 'rxjs/operators';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService) { }

  private setToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({ setHeaders: { Authorization: token } });
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return this.auth.getAuthorizationHeaderValue().pipe(

      mergeMap(token => {
        return next.handle(this.setToken(req, token));
      }),

      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.auth.renewToken().pipe(
            mergeMap(newToken => next.handle(this.setToken(req, newToken)))
          );
        } else {
          return throwError(error);
        }
      })

    );
  }
}
