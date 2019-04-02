import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { Observable } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

// todo: handle 401 error
@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return this.auth.getAuthorizationHeaderValue().pipe(
      mergeMap(header => {
        req = req.clone({ setHeaders: { Authorization: header } });
        return next.handle(req);
      })
    );
  }
}
