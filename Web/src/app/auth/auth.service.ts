import { Injectable } from '@angular/core';
import { UserManager, UserManagerSettings, User } from 'oidc-client';
import { BehaviorSubject, Observable, from, Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import * as Oidc from 'oidc-client';

export { User };

export class Permission {
  public id: number;
  public name: string;
  public description: string;
}

export class UserProfile {
  public sub: string;
  public id: number;
  public isGlobal: boolean;
  public permissions: Permission[];
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userManager: UserManager;
  private user: User;
  private userProfile: UserProfile;

  private userSignedInSubject = new Subject<User>();
  private userSignedOutSubject = new BehaviorSubject<void>(null);

  public readonly userSignedIn = this.userSignedInSubject.asObservable();
  public readonly userSignedOut = this.userSignedOutSubject.asObservable();

  private readonly backendUrl = 'http://localhost:4200';

  constructor(private httpClient: HttpClient) {

    Oidc.Log.logger = console;
    Oidc.Log.level = Oidc.Log.INFO;

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

    this.userManager.events.addUserLoaded((user: User) => {
      console.log('addUserLoaded', user.access_token);
      this.user = user;
    });
  }

  getAuthorizationHeaderValue(): string {
    return `${this.user.token_type} ${this.user.access_token}`;
  }

  isLoggedIn(): boolean {
    return this.user != null;
  }

  private loadProfile(sub: string): Observable<UserProfile> {
    return this.httpClient.get<UserProfile>(`https://localhost:5101/api/users`);
  }

  private loadUser(): Observable<boolean> {

    const ret = new Promise<boolean>((resolve, reject) => {
      this.userManager.getUser().then(user => {

        if (!user) {
          resolve(false);
          return;
        }

        console.log('loadUser.getUser', user.profile.name);
        this.user = user;

        if (user && !this.userProfile) {
          const sub = user.profile.sub;
          this.loadProfile(sub).subscribe(profile => {
            console.log('loadUser.getUser.loadProfile', profile);
            if (profile) {
              this.userProfile = profile;
              this.userSignedInSubject.next(user);
              resolve(true);
            } else {
              resolve(false);
            }
          },
            loadProfileReason => reject(loadProfileReason));
        } else {
          resolve(false);
        }
      },
        getUserReason => reject(getUserReason));
    });

    return from(ret);
  }

  isLoggedInObs(): Observable<boolean> {
    return this.loadUser().pipe(map(() => {
      if (this.user && this.user.access_token && this.userProfile) {
        return true;
      } else {
        return false;
      }
    }));
  }

  startAuthentication(returnUrl: string): Promise<void> {
    return this.userManager.signinRedirect({
      data: { rediectUrl: returnUrl }
    });
  }

  public competeSignIn(): Promise<string> {
    return new Promise<string>((resolve, reject) => {
      this.userManager.signinRedirectCallback().then(user => {
        console.log('signinRedirectCallback.then', user.profile.name);
        this.user = user;
        this.loadProfile(user.profile.sub).subscribe(profile => {
          console.log('competeSignIn', profile);

          this.userProfile = profile;
          this.userSignedInSubject.next(user);

          resolve(user.state.redirectUrl);
        }, err => reject(err));
      });
    });
  }

  public signOut(): Promise<void> {
    this.userSignedOutSubject.next();
    return this.userManager.signoutRedirect();
  }

  public hasPermission(permissionName: string): boolean {
    if (this.userProfile.isGlobal) {
      return true;
    }

    return this.userProfile.permissions.some(el => el.name === permissionName);
  }
}
