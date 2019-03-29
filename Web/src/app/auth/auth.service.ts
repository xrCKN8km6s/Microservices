import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { BehaviorSubject, Observable, from, Subject } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

export { User };

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userManager: UserManager;
  private userProfile: UserProfile;

  private userSignedInSubject = new Subject<User>();
  private userSignedOutSubject = new BehaviorSubject<void>(null);

  public readonly userSignedIn = this.userSignedInSubject.asObservable();
  public readonly userSignedOut = this.userSignedOutSubject.asObservable();

  private readonly backendUrl = 'http://localhost:4200';

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
  }

  getAuthorizationHeaderValue(): Observable<string> {
    return from(this.userManager.getUser()).pipe(
      map(user => `${user.token_type} ${user.access_token}`)
    );
  }

  private loadProfile(): Observable<UserProfile> {
    return this.httpClient.get<UserProfile>(`https://localhost:5101/api/users/profile`);
  }

  isLoggedInObs(): Observable<boolean> {
    return from(this.userManager.getUser()).pipe(
      map(user => {
        return !!user && !!user.access_token && !!this.userProfile;
      })
    );
  }

  startAuthentication(url: string): Promise<void> {
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
    if (this.userProfile.hasGlobalRole) {
      return true;
    }
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
