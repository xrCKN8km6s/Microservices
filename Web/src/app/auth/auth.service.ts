import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { Observable, from, Subject } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import * as Oidc from 'oidc-client';

export { User };

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly backendUrl = 'http://localhost:4200';

  private userManager: UserManager;
  private userProfile: UserProfile;

  private userSignedInSubject = new Subject<User>();
  public readonly userSignedIn = this.userSignedInSubject.asObservable();

  private userSignedOutSubject = new Subject();
  public readonly userSignedOut = this.userSignedOutSubject.asObservable();

  constructor(private httpClient: HttpClient) {
    this.userManager = new UserManager({
      authority: 'http://localhost:3000',
      client_id: 'spa',
      redirect_uri: `${this.backendUrl}/signin-callback`,
      silent_redirect_uri: `${this.backendUrl}/assets/silent-callback.html`,
      post_logout_redirect_uri: `${this.backendUrl}`,
      response_type: 'code',
      scope: 'openid profile email orders users',
      automaticSilentRenew: true
    });

    Oidc.Log.logger = console;
  }

  private getUser(): Observable<User> {
    return from(this.userManager.getUser());
  }

  private loadProfile(): Observable<UserProfile> {
    return this.httpClient.get<UserProfile>(`https://localhost:5101/api/users/profile`);
  }

  public getAuthorizationHeaderValue(): Observable<string> {
    return this.getUser().pipe(
      map(user => `${user.token_type} ${user.access_token}`)
    );
  }

  public isLoggedIn(): Observable<boolean> {
    return this.getUser().pipe(
      map(user => {
        return !!user && !!user.access_token && !!this.userProfile;
      })
    );
  }

  public startAuthentication(url: string): Promise<void> {
    return this.userManager.signinRedirect({
      data: { returnUrl: url }
    });
  }

  public competeSignIn(): Observable<string> {
    return from(this.userManager.signinRedirectCallback()).pipe(
      mergeMap(user => {
        return this.loadProfile().pipe(map(profile => {
          this.userProfile = profile;
          this.userSignedInSubject.next(user);
          return user.state.returnUrl;
        }));
      }));
  }

  public signOut(): Promise<void> {
    this.userSignedOutSubject.next();
    return this.userManager.signoutRedirect();
  }

  public hasPermission(permissionName: string): boolean {
    return this.userProfile.permissions.some(el => el.name === permissionName);
  }
}

class UserProfile {
  public sub: string;
  public id: number;
  public hasGlobalRole: boolean;
  public permissions: Permission[];
}

class Permission {
  public id: number;
  public name: string;
  public description: string;
}
