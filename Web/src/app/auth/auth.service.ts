import { Injectable } from '@angular/core';
import { UserManager, UserManagerSettings, User } from 'oidc-client';
import { BehaviorSubject, Observable, from } from 'rxjs';
import { map } from 'rxjs/operators';

export { User };

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userManager: UserManager;
  private user: User;
  private userSignedInSubject = new BehaviorSubject<User>(null);
  private userSignedOutSubject = new BehaviorSubject<void>(null);

  public readonly userSignedIn = this.userSignedInSubject.asObservable();
  public readonly userSignedOut = this.userSignedOutSubject.asObservable();

  private readonly backendUrl = 'http://localhost:4200';

  constructor() {

    this.userManager = new UserManager({
      authority: 'http://localhost:3000',
      client_id: 'EntryPoint',
      redirect_uri: `${this.backendUrl}/signin-callback`,
      silent_redirect_uri: `${this.backendUrl}/silent-callback`,
      post_logout_redirect_uri: `${this.backendUrl}`,
      response_type: 'id_token token',
      scope: 'openid profile email EntryPoint.rw',
      automaticSilentRenew: true
    });
  }

  getAuthorizationHeaderValue(): string {
    return `${this.user.token_type} ${this.user.access_token}`;
  }

  isLoggedIn(): boolean {
    return this.user != null;
  }

  isLoggedInObs(): Observable<boolean> {
    return from(this.userManager.getUser()).pipe(map(user => {

      if (!user) {
        return false;
      }

      this.user = user;
      this.userSignedInSubject.next(user);
      return true;
    }));
  }

  startAuthentication(returnUrl: string): Promise<void> {
    return this.userManager.signinRedirect({
      data: { rediectUrl: returnUrl }
    });
  }

  public competeSignIn(): Promise<any> {
    return this.userManager.signinRedirectCallback().then(user => {
      this.user = user;
      this.userSignedInSubject.next(user);
      return user.state;
    });
  }

  public renewToken(): Promise<User> {
    return this.userManager.signinSilentCallback();
  }

  public signOut(): Promise<void> {
    this.userSignedOutSubject.next();
    return this.userManager.signoutRedirect();
  }
}
