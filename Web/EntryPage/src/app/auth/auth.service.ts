import { Injectable } from '@angular/core';
import { UserManager, UserManagerSettings, User } from 'oidc-client';
import { BehaviorSubject } from 'rxjs';

export { User };

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private user: User;
  private userSignedInSubject = new BehaviorSubject<User>(null);
  private userSignedOutSubject = new BehaviorSubject<void>(null);

  public readonly userSignedIn = this.userSignedInSubject.asObservable();
  public readonly userSignedOut = this.userSignedOutSubject.asObservable();

  userManager: UserManager;
  path = 'http://localhost:4200';

  constructor() {
    const settings = {
      authority: 'http://localhost:3000',
      client_id: 'EntryPoint',
      redirect_uri: `${this.path}/signin-callback`,
      silent_redirect_uri: `${this.path}/silent-callback`,
      post_logout_redirect_uri: `${this.path}`,
      response_type: 'id_token token',
      scope: 'openid profile email EntryPoint.rw',
      automaticSilentRenew: true
    };

    this.userManager = new UserManager(settings);
  }

  public getUser(): Promise<User> {
    return this.userManager.getUser().then(user => {
      this.user = user;
      this.userSignedInSubject.next(user);

      return user;
    });
  }

  getAuthorizationHeaderValue(): string {
    return `${this.user.token_type} ${this.user.access_token}`;
  }

  isLoggedIn(): boolean {
    return this.user != null && !this.user.expired;
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
