import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule, Component, OnInit, Injectable } from '@angular/core';
import { AppService } from './app.service';

import { AppComponent } from './app.component';

import { RouterModule, Routes, Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';


import { AuthService } from './auth/auth.service';

import { HomeComponent } from './home/home.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatBadgeModule } from '@angular/material/badge';



import { MainMenuComponent } from './main-menu/main-menu.component';


@Injectable()
export class AuthGuard implements CanActivate {

  constructor(private auth: AuthService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

    if (this.auth.isLoggedIn()) {
      return true;
    }

    this.auth.startAuthentication(state.url);
    return false;
  }
}


@Component({
  template: ''
})
export class SignInCallbackComponent implements OnInit {

  constructor(private auth: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.auth.competeSignIn().then(state => {
      this.router.navigateByUrl(state.redirectUrl);
    });
  }
}

@Component({
  template: ''
})
export class SilentCallbackComponent implements OnInit {

  constructor(private auth: AuthService) { }

  ngOnInit(): void {
    this.auth.renewToken();
  }
}


@NgModule({
  declarations: [
    AppComponent,
    SignInCallbackComponent,
    SilentCallbackComponent,
    HomeComponent,
    MainMenuComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot(
      [
        { path: 'signin-callback', component: SignInCallbackComponent },
        { path: 'silent-callback', component: SilentCallbackComponent },
        { path: 'home', pathMatch: 'full', component: HomeComponent, canActivate: [AuthGuard] },
        { path: '', pathMatch: 'full', redirectTo: '/home' }
      ],
      { enableTracing: false }
    ),
    BrowserAnimationsModule,
    MatMenuModule,
    MatButtonModule,
    MatIconModule,
    MatToolbarModule,
    MatBadgeModule
  ],
  providers: [
    AppService,
    AuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }







