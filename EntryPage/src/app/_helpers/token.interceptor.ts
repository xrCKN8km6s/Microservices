import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { Observable } from 'rxjs';


@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (!this.auth.isLoggedIn()) {
      return next.handle(req);
    }

    const token = this.auth.getAuthorizationHeaderValue();

    if (token) {
      req = req.clone(
        {
          setHeaders:
          {
            Authorization: token
          }
        }
      );
    }

    return next.handle(req);
  }
}
