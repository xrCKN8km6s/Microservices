import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private auth: AuthService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {

    return this.auth.isLoggedInObs().pipe(map(loggedIn => {
      if (loggedIn) {
        return true;
      } else {
        const returnUrl = state.url;
        this.auth.startAuthentication(returnUrl);
        return false;
      }
    }));
  }
}
