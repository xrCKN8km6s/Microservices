import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { Observable, from, Subject, of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import * as Oidc from 'oidc-client';
import { environment } from 'src/environments/environment';
import { UserProfile } from './user-profile';
import { UserProfileService } from './user-profile.service';

export { User };

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly profilePath = 'api/profile';

  private userManager: UserManager;

  private userSignedInSubject = new Subject<User>();
  public readonly userSignedIn = this.userSignedInSubject.asObservable();

  private userSignedOutSubject = new Subject();
  public readonly userSignedOut = this.userSignedOutSubject.asObservable();

  constructor(private httpClient: HttpClient, private userProfileService: UserProfileService) {
    this.userManager = new UserManager({
      authority: environment.authorityUrl,
      client_id: environment.clientId,
      redirect_uri: `${environment.webUrl}/signin-callback`,
      silent_redirect_uri: `${environment.webUrl}/assets/silent-callback.html`,
      post_logout_redirect_uri: environment.webUrl,
      response_type: 'code',
      scope: environment.scope,
      automaticSilentRenew: true
    });

    Oidc.Log.logger = console;
  }

  private getUser(): Observable<User> {
    return from(this.userManager.getUser());
  }

  private loadProfile(user: User): Observable<UserProfile> {
    return this.httpClient.get<UserProfile>(`${environment.bffUrl}/${this.profilePath}`).pipe(map(profile => {
      this.userProfileService.setProfile(profile);
      this.userSignedInSubject.next(user);
      return profile;
    }));
  }

  private formatHeader(user: User): string {
    return `${user.token_type} ${user.access_token}`;
  }

  public getAuthorizationHeaderValue(): Observable<string> {
    return this.getUser().pipe(
      map(this.formatHeader)
    );
  }

  public renewToken(): Observable<string> {
    return from(this.userManager.signinSilent()).pipe(map(this.formatHeader));
  }

  public isLoggedIn(): Observable<boolean> {
    return this.getUser().pipe(
      mergeMap(user => {
        return this.loadProfile(user).pipe(map(_ => {
          return !!user && !!user.access_token && this.userProfileService.hasProfile();
        }));
      }),
      catchError(_ => of(false))
    );
  }

  public startAuthentication(url: string): void {
    this.userManager.signinRedirect({
      data: { returnUrl: url }
    });
  }

  public competeSignIn(): Observable<string> {
    return from(this.userManager.signinRedirectCallback()).pipe(
      mergeMap(user => {
        return this.loadProfile(user).pipe(map(_ => {
          return user.state.returnUrl;
        }));
      }));
  }

  public signOut(): Promise<void> {
    this.userSignedOutSubject.next();
    return this.userManager.signoutRedirect();
  }
}
